using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
	
	private string _assetsNamePrefix;
	
	private Vector2 _scrollPos;

	private List<UnityEngine.Object> _assets = new();
	private List<string>             _issues = new();


	private void OnGUI()
	{
		EditorGUILayout.BeginHorizontal();
		
		_assetsNamePrefix = EditorGUILayout.TextField("Assets Names Prefix", _assetsNamePrefix);

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
		string   folderPath  = AssetDatabase.GUIDToAssetPath(_selectedGuid);
		string[] assetsPaths = Directory.GetFiles(folderPath, "*.asset", SearchOption.AllDirectories);

		_assets.Clear();
		foreach (string path in assetsPaths)
			_assets.AddRange(AssetDatabase.LoadAllAssetsAtPath(path));

		_issues.Clear();
		foreach (var asset in _assets)
			CheckObj(asset, asset);
	}

	private bool CheckObj(UnityEngine.Object rootUnityObj, object obj)
	{
		var type = obj.GetType();
		if (!ReferenceEquals(rootUnityObj, obj) && type.IsSubclassOf(typeof(ScriptableObject)))
		{
			var unityObj = (UnityEngine.Object)obj;
			if (!string.IsNullOrWhiteSpace(_assetsNamePrefix) && unityObj.name.StartsWith(_assetsNamePrefix))
				return _assets.Contains(unityObj);
			return true;
		}

		while (type != null && type.FullName?.StartsWith("System.") != true)
		{
			var fields = obj.GetType().GetFields(BindingFlags.Instance |
			                                     BindingFlags.Public |
			                                     BindingFlags.NonPublic
			);

			foreach (var field in fields)
			{
				object fieldValue = field.GetValue(obj);
				if (fieldValue == null)
					continue;
				if (field.FieldType.IsArray)
				{
					foreach (object fieldValueArrayItem in (Array)fieldValue)
						if (fieldValueArrayItem != null)
						{
							if (!CheckObj(rootUnityObj, fieldValueArrayItem))
							{
								var unityObj = (UnityEngine.Object)fieldValueArrayItem;
								_issues.Add($"{AssetDatabase.GetAssetPath(rootUnityObj)} | " +
								            $"{field.Name} | {AssetDatabase.GetAssetPath(unityObj)}"
								);
							}
						}
				}
				else
				{
					if (!CheckObj(rootUnityObj, fieldValue))
					{
						var unityObj = (UnityEngine.Object)fieldValue;
						_issues.Add($"{AssetDatabase.GetAssetPath(rootUnityObj)} | " +
						            $"{field.Name} | {AssetDatabase.GetAssetPath(unityObj)}"
						);
					}
				}
			}

			type = type.BaseType;
		}

		return true;
	}
}
}