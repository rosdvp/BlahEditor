using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BlahEditor.Editor.BatchRename
{
public class EditorBatchRenameWindow : EditorWindow
{
	[MenuItem("Assets/Batch Rename")]
	private static void BatchRename()
	{
		var window = GetWindow<EditorBatchRenameWindow>();
		window._selectedGuids = Selection.assetGUIDs;
		window.Show();
	}

	//-----------------------------------------------------------
	//-----------------------------------------------------------
	private string[]        _selectedGuids;
	private bool            _withFolders;
	private bool            _withFoldersScan;
	private HashSet<string> _paths;
	private string          _from;
	private string          _to;
	private Vector2         _scrollPos;

	private void OnGUI()
	{
		var styleRightAlign = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleRight };
		
		if (_paths == null)
			FillPaths();
		
		bool prevWithFolders     = _withFolders;
		bool prevWithFoldersScan = _withFoldersScan;
		EditorGUILayout.BeginHorizontal();
		_withFolders = EditorGUILayout.Toggle(GUIContent.none, _withFolders, GUILayout.MaxWidth(20));
		EditorGUILayout.LabelField("Rename folders");
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		_withFoldersScan = EditorGUILayout.Toggle(GUIContent.none, _withFoldersScan, GUILayout.MaxWidth(20));
		EditorGUILayout.LabelField("Include assets from folders");
		EditorGUILayout.EndHorizontal();
		if (prevWithFolders != _withFolders || prevWithFoldersScan != _withFoldersScan)
			FillPaths();

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
			FillPaths();
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
		EditorGUILayout.LabelField("Preview:");

		_scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
		foreach (string path in _paths)
		{
			string nameBefore = path.Split('/')[^1];
			string nameAfter = IsSettingsValid
				? nameBefore.Replace(_from, _to)
				: nameBefore;

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(nameBefore);
			EditorGUILayout.LabelField(nameAfter);
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndScrollView();
	}

	private bool IsSettingsValid => !string.IsNullOrEmpty(_from) && _to != null;

	private void Rename()
	{
		if (_paths == null || _paths.Count == 0)
			throw new Exception("Failed to rename: no assets in selected folder detected");
		if (!IsSettingsValid)
			throw new Exception("Failed to rename: settings are invalid");
		
		foreach (string path in _paths)
		{
			string nameBefore = path.Split('/')[^1];
			string nameAfter  = nameBefore.Replace(_from, _to);
			AssetDatabase.RenameAsset(path, nameAfter);
		}
	}

	//-----------------------------------------------------------
	//-----------------------------------------------------------
	private void FillPaths()
	{
		_paths = new HashSet<string>();
		foreach (string guid in _selectedGuids)
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			if (Directory.Exists(path))
			{
				if (_withFolders)
					AddAssetPath(path);
				if (_withFoldersScan)
					FillPathsFromFolderScan(path);
			}
			else
				AddAssetPath(path);
		}
	}

	private void FillPathsFromFolderScan(string folderPath)
	{
		foreach (string path in Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories))
			if (!path.EndsWith(".meta"))
			{
				if (Directory.Exists(path))
				{
					if (_withFolders)
						AddAssetPath(path);
					if (_withFoldersScan)
						FillPathsFromFolderScan(path);
				}
				else
					AddAssetPath(path);
			}
	}
	
	private void AddAssetPath(string path)
	{
		_paths.Add(path.Replace('\\', '/'));
	}
}
}