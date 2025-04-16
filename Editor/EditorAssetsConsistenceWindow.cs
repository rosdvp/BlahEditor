using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BlahEditor.Editor
{
public class EditorAssetsConsistenceWindow : EditorWindow
{
	[MenuItem("Assets/Blah/Check Assets Consistence")]
	private static void ShowWindow()
	{
		var window = GetWindow<EditorAssetsConsistenceWindow>();
		window._selectedGuid = Selection.assetGUIDs[0];
		window.Show();
	}

	//-----------------------------------------------------------
	//-----------------------------------------------------------
	private string _selectedGuid;
	
	private string _refAssetNamePrefix;
	
	private Vector2 _scrollPos;

	private List<string> _issues = new();


	private void OnGUI()
	{
		EditorGUILayout.BeginHorizontal();
		
		_refAssetNamePrefix = EditorGUILayout.TextField("Reference Asset Name Prefix", _refAssetNamePrefix);

		if (GUILayout.Button("Check"))
			Check();
		
		EditorGUILayout.EndHorizontal();
		
		_scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
		
		foreach (string issue in _issues)
			EditorGUILayout.LabelField(issue);

		EditorGUILayout.EndScrollView();
	}


	private void Check()
	{
		_issues.Clear();
		
		string folderPath = AssetDatabase.GUIDToAssetPath(_selectedGuid);

		string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { folderPath });
		foreach (string guid in guids)
		{
			string soPath = AssetDatabase.GUIDToAssetPath(guid);
			var    so   = AssetDatabase.LoadAssetAtPath<ScriptableObject>(soPath);

			using var ser  = new SerializedObject(so);
			var       iter = ser.GetIterator();
			iter.NextVisible(true);  // skip Base
			iter.NextVisible(false); // skip m_Script
			do
			{
				if (iter.propertyType != SerializedPropertyType.ObjectReference)
					continue;
				
				if (!string.IsNullOrWhiteSpace(_refAssetNamePrefix) && 
				    !iter.objectReferenceValue.name.StartsWith(_refAssetNamePrefix))
					continue;

				string refObjPath = AssetDatabase.GetAssetPath(iter.objectReferenceValue);
				if (refObjPath.StartsWith(folderPath))
					continue;
				
				_issues.Add($"{soPath} | {iter.propertyPath} | {refObjPath}");
			}
			while (iter.NextVisible(true));
		}
	}
}
}