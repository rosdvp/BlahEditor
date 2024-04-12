using System;
using System.Collections.Generic;
using System.Reflection;
using BlahEditor.DrawersExtensions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

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
		ValidatePropRef(prop);

		var rectHeader = rect.ToSingleLine();
		var rectsHeader = string.IsNullOrEmpty(label.text) // to support EnumDict
			? rectHeader.SplitHorizontal(5f, 0.001f, 0.76f, 0.12f, 0.12f)
			: rectHeader.SplitHorizontal(5f, 0.4f, 0.36f, 0.12f, 0.12f);
		
		EditorGUI.LabelField(rectsHeader[0], label.text);
		EditorGUI.BeginDisabledGroup(true);
		EditorGUI.ObjectField(rectsHeader[1], prop, GUIContent.none);
		EditorGUI.EndDisabledGroup();
		
		if (prop.objectReferenceValue == null)
		{
			if (GUI.Button(rectsHeader[2], "sel"))
				OnSelectTap(prop);
			if (GUI.Button(rectsHeader[3], "add"))
				OnAddTap(prop);
		}
		else
		{
			if (IsInlineSO())
			{
				var rectInner = rect.ReduceFromTop(
					rect.ToSingleLine().height + EditorGUIUtility.standardVerticalSpacing
				);
				EditorGUI.PropertyField(rectInner, prop, GUIContent.none);
			}

			if (GUI.Button(rectsHeader[2].CombineAtRight(rectsHeader[3]), "rem"))
				OnRemoveTap(prop);
		}
		
		prop.serializedObject.ApplyModifiedProperties();
	}

	public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
	{
		float height = EditorGUIUtility.singleLineHeight;
		if (prop.objectReferenceValue != null && IsInlineSO())
			height += EditorGUIUtility.standardVerticalSpacing +
			          EditorGUI.GetPropertyHeight(prop, GUIContent.none);
		return height;
	}

	private bool IsInlineSO()
	{
		return fieldInfo.GetCustomAttribute<InlineSOAttribute>() != null;
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
				prop.objectReferenceValue = null;
		}
	}


	private void OnAddTap(SerializedProperty prop)
	{
		var menu = new GenericMenu();
		foreach (var type in Types)
			menu.AddItem(
				new GUIContent(type.Name),
				false,
				() => OnCreateTap(prop, type)
			);
		menu.ShowAsContext();
	}

	private void OnSelectTap(SerializedProperty prop)
	{
		var menu = new GenericMenu();
		foreach (var subAsset in FindSubAssets(prop))
			menu.AddItem(
				new GUIContent(subAsset.name),
				false,
				() =>
				{
					subAsset.name = BuildSubAssetName(prop);
				}
			);
		menu.ShowAsContext();
	}
    
	private IEnumerable<Object> FindSubAssets(SerializedProperty prop)
	{
		string path      = AssetDatabase.GetAssetPath(prop.serializedObject.targetObject);
		var    subAssets = AssetDatabase.LoadAllAssetsAtPath(path);
		foreach (var subAsset in subAssets)
			if (subAsset != prop.serializedObject.targetObject)
				yield return subAsset;
	}

	private void OnCreateTap(SerializedProperty prop, Type type)
	{
		var so = ScriptableObject.CreateInstance(type);
		so.name = BuildSubAssetName(prop);

		AssetDatabase.AddObjectToAsset(so, prop.serializedObject.targetObject);
		EditorUtility.SetDirty(so);
		AssetDatabase.SaveAssetIfDirty(so);
	}

	private void OnRemoveTap(SerializedProperty prop)
	{
		AssetDatabase.RemoveObjectFromAsset(prop.objectReferenceValue);
		prop.objectReferenceValue = null;
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