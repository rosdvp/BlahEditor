using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BlahEditor.Attributes.Editor
{
/// <summary>
/// Inherit this drawer and make an attribute for your own sub-assets.
/// </summary>
public abstract class SubAssetDrawer : PropertyDrawer
{
	protected abstract IReadOnlyList<Type> Types      { get; }

	public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
	{
		var rects = string.IsNullOrEmpty(label.text)
			? rect.SplitHorizontal(5f, 0.001f, 0.76f, 0.12f, 0.12f)
			: rect.SplitHorizontal(5f, 0.4f, 0.36f, 0.12f, 0.12f);

		EditorGUI.LabelField(rects[0], label.text);
		
		EditorGUI.BeginDisabledGroup(true);
		ValidatePropRef(prop);
		EditorGUI.PropertyField(rects[1], prop, GUIContent.none);
		EditorGUI.EndDisabledGroup();

		if (prop.objectReferenceValue == null)
		{
			if (GUI.Button(rects[2], "sel"))
			{
				var menu = new GenericMenu();
				foreach (var subAsset in FindSubAssets(prop))
					menu.AddItem(new GUIContent(subAsset.name),
					             false,
					             () =>
					             {
						             prop.objectReferenceValue = subAsset;
						             prop.serializedObject.ApplyModifiedProperties();
					             }
					);
				menu.ShowAsContext();
			}
			if (GUI.Button(rects[3], "add"))
			{
				var menu = new GenericMenu();
				foreach (var type in Types)
					menu.AddItem(new GUIContent(type.Name),
					             false,
					             () => Create(prop, type)
					);
				menu.ShowAsContext();
			}
		}
		else
		{
			if (GUI.Button(rects[2], "sel"))
				prop.objectReferenceValue = null;
			if (GUI.Button(rects[3], "rem"))
				Remove(prop);
		}
	}
	
	public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
	{
		return EditorGUI.GetPropertyHeight(prop);
	}


	private void ValidatePropRef(SerializedProperty prop)
	{
		string assetName = BuildSubAssetName(prop);

		if (prop.objectReferenceValue == null)
		{
			string path   = AssetDatabase.GetAssetPath(prop.serializedObject.targetObject);
			var    assets = AssetDatabase.LoadAllAssetsAtPath(path);
			var    asset  = Array.Find(assets, a => a.name == assetName);
			if (asset != null)
				prop.objectReferenceValue = asset;
		}
		else
		{
			var asset = prop.objectReferenceValue;
			if (asset.name != assetName)
			{
				asset.name = assetName;

				//just to refresh Unity UI
				string path    = AssetDatabase.GetAssetPath(prop.serializedObject.targetObject);
				var    tempObj = new TextAsset();
				AssetDatabase.AddObjectToAsset(tempObj, path);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
				AssetDatabase.RemoveObjectFromAsset(tempObj);
			}
		}
	}


	private IEnumerable<UnityEngine.Object> FindSubAssets(SerializedProperty prop)
	{
		string path      = AssetDatabase.GetAssetPath(prop.serializedObject.targetObject);
		var    subAssets = AssetDatabase.LoadAllAssetsAtPath(path);
		foreach (var subAsset in subAssets)
			if (subAsset != prop.serializedObject.targetObject)
				yield return subAsset;
	}

	private void Create(SerializedProperty prop, Type type)
	{
		var so = ScriptableObject.CreateInstance(type);
		so.name = BuildSubAssetName(prop);

		AssetDatabase.AddObjectToAsset(so, prop.serializedObject.targetObject);
		AssetDatabase.SaveAssets();
	}

	private void Remove(SerializedProperty prop)
	{
		AssetDatabase.RemoveObjectFromAsset(prop.objectReferenceValue);

		prop.objectReferenceValue = null;

		AssetDatabase.SaveAssets();
	}

	private string BuildSubAssetName(SerializedProperty prop)
	{
		string[] rawParentName = prop.serializedObject.targetObject.name.Split('&');
		string   parentName    = rawParentName.Length > 1 ? rawParentName[^1] : null;
		var      assetName     = $"{parentName}&{prop.propertyPath}";

		return assetName;
	}
}
}