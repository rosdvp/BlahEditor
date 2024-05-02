using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace BlahEditor.Editor
{
public class EditorBatchRenameWindow : EditorWindow
{
	[MenuItem("Assets/Blah/Batch Rename")]
	private static void BatchRename()
	{
		var window = GetWindow<EditorBatchRenameWindow>();
		window._selectedGUIDs = Selection.assetGUIDs;
		window.FillPathsFromSelected();
		
		window.Show();
	}

	//-----------------------------------------------------------
	//-----------------------------------------------------------
	private string[] _selectedGUIDs;

	private GUIStyle _styleNoChanges;
	private GUIStyle _styleNameBefore;
	private GUIStyle _styleNameAfter;
	
	private bool    _isIncludeFoldersNames   = true;
	private bool    _isIncludeFoldersContent = true;
	private bool    _isToLowerSnakeCase      = false;
	private string  _from;
	private string  _to;
	private Vector2 _scrollPos;
	
	private HashSet<string> _paths = new();

	private void OnGUI()
	{
		if (_styleNoChanges == null)
		{
			_styleNoChanges                   = new GUIStyle(EditorStyles.label);
			_styleNameBefore                  = new GUIStyle(EditorStyles.label);
			_styleNameBefore.normal.textColor = new Color(0.7f, 0, 0);
			_styleNameAfter                   = new GUIStyle(EditorStyles.label);
			_styleNameAfter.normal.textColor  = new Color(0, 0.7f, 0);
		}
		
		//var styleRightAlign = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleRight };

		bool prevIncludeFoldersNames   = _isIncludeFoldersNames;
		bool prevIncludeFoldersContent = _isIncludeFoldersContent;
		EditorGUILayout.BeginHorizontal();
		_isIncludeFoldersNames = EditorGUILayout.Toggle(
			GUIContent.none,
			_isIncludeFoldersNames,
			GUILayout.MaxWidth(20)
		);
		EditorGUILayout.LabelField("Rename folders");
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		_isIncludeFoldersContent = EditorGUILayout.Toggle(
			GUIContent.none,
			_isIncludeFoldersContent,
			GUILayout.MaxWidth(20)
		);
		EditorGUILayout.LabelField("Include assets from folders");
		EditorGUILayout.EndHorizontal();
		if (prevIncludeFoldersNames != _isIncludeFoldersNames ||
		    prevIncludeFoldersContent != _isIncludeFoldersContent)
		{
			FillPathsFromSelected();
		}
		
		EditorGUILayout.BeginHorizontal();
		_isToLowerSnakeCase = EditorGUILayout.Toggle(
			GUIContent.none,
			_isToLowerSnakeCase,
			GUILayout.MaxWidth(20)
		);
		EditorGUILayout.LabelField("To lower_snake_case");
		EditorGUILayout.EndHorizontal();

		if (GUILayout.Button("Scan"))
		{
			_selectedGUIDs = Selection.assetGUIDs;
			FillPathsFromSelected();
		}

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("From", GUILayout.MaxWidth(50));
		_from = EditorGUILayout.TextField(GUIContent.none, _from, GUILayout.MaxWidth(150));
		EditorGUILayout.Space(10);
		EditorGUILayout.LabelField("To", GUILayout.MaxWidth(50));
		_to   = EditorGUILayout.TextField(GUIContent.none, _to, GUILayout.MaxWidth(150));
		EditorGUILayout.Space(10);
		if (GUILayout.Button("Rename"))
		{
			Rename();
			FillPathsFromSelected();
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
		EditorGUILayout.LabelField("Preview:");
		_scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
		foreach (string path in _paths)
		{
			(string before, string after) = Rename(path);

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(before, before != after ? _styleNameBefore : _styleNoChanges);
			EditorGUILayout.LabelField(after, before != after ? _styleNameAfter : _styleNoChanges);
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndScrollView();
	}

	private void Rename()
	{
		foreach (string path in _paths)
		{
			(string before, string after) = Rename(path);
			AssetDatabase.RenameAsset(path, after);
		}
	}

	private (string nameBefore, string nameAfter) Rename(string path)
	{
		string nameBefore = path.Split('/')[^1];
		string nameAfter = !string.IsNullOrEmpty(_from) && !string.IsNullOrEmpty(_from)
			? nameBefore.Replace(_from, _to)
			: nameBefore;

		if (_isToLowerSnakeCase)
			nameAfter = Regex.Replace(
				nameAfter,
				"(?<!^)(([A-Z][a-z])|([A-Z][A-Z]))",
				"_$1"
			).ToLower().Replace("__", "_");
		return (nameBefore, nameAfter);
	}
	
	//-----------------------------------------------------------
	//-----------------------------------------------------------
	private void FillPathsFromSelected()
	{
		_paths.Clear();
		
		foreach (string guid in _selectedGUIDs)
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			if (Directory.Exists(path))
			{
				if (_isIncludeFoldersNames)
					AddPath(path);
				if (_isIncludeFoldersContent)
					FillPathsFromFolderScan(path);
			}
			else
				AddPath(path);
		}
	}

	private void FillPathsFromFolderScan(string folderPath)
	{
		foreach (string path in Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories))
			if (!path.EndsWith(".meta"))
			{
				if (Directory.Exists(path))
				{
					if (_isIncludeFoldersNames)
						AddPath(path);
					if (_isIncludeFoldersContent)
						FillPathsFromFolderScan(path);
				}
				else
					AddPath(path);
			}
	}
	
	private void AddPath(string path)
	{
		_paths.Add(path.Replace('\\', '/'));
	}
}
}