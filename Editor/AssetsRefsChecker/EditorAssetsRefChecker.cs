using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BlahEditor.Editor.AssetsRefsChecker
{
public static class EditorAssetsRefCheckerWindow
{
	[MenuItem("Assets/Check Assets Refs Consistent")]
	private static void Check()
	{
		string   folderGuid  = Selection.assetGUIDs[0];
		string   folderPath  = AssetDatabase.GUIDToAssetPath(folderGuid);
		string[] assetsPaths = Directory.GetFiles(folderPath, "*.asset", SearchOption.AllDirectories);

		var assets = new List<object>();
		foreach (string path in assetsPaths)
			assets.Add(AssetDatabase.LoadAssetAtPath<ScriptableObject>(path));

		foreach (object asset in assets)
			CheckFields(assets, asset);

		Debug.Log("Check done");
	}


	private static void CheckFields(List<object> allowedAssets, object asset)
	{
		var fields = asset.GetType().GetFields(BindingFlags.Instance |
		                                       BindingFlags.Public |
		                                       BindingFlags.NonPublic);
		foreach (var field in fields)
		{
			object value = field.GetValue(asset);
			if (value == null)
				continue;
			if (field.FieldType.IsArray)
			{
				foreach (object item in (Array)value)
					if (item != null)
					{
						if (item.GetType().IsSubclassOf(typeof(ScriptableObject)))
						{
							if (!allowedAssets.Contains(item))
								Debug.Log($"{asset};\nfield {field.Name} looks suspicious");
						}
						else
							CheckFields(allowedAssets, item);
					}
				continue;
			}
			if (!field.FieldType.IsClass)
				continue;

			if (field.FieldType.IsSubclassOf(typeof(ScriptableObject)))
			{
				if (!allowedAssets.Contains(value))
					Debug.Log($"{asset};\nfield {field.Name} looks suspicious");
			}
			else
				CheckFields(allowedAssets, field.GetValue(asset));
		}
	}
}
}