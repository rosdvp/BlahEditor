using System;
using System.Collections.Generic;
using System.Reflection;
using BlahEditor.DrawersExtensions;
using UnityEditor;
using UnityEngine;

namespace BlahEditor.Attributes.Editor
{
[CustomPropertyDrawer(typeof(InlineSOAttribute))]
public class InlineSODrawer : PropertyDrawer
{
	public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
	{
		bool isEditable = ((InlineSOAttribute)attribute).IsEditable;

		rect = rect.ToSingleLine();
		EditorGUI.PropertyField(rect, prop, label);
		rect = rect.ToNextLine();

		bool prevEnabled = GUI.enabled;
		GUI.enabled = isEditable;
		rect        = rect.ReduceFromLeft(10);
		foreach (var p in EnumerateContent(prop))
		{
			rect.height = EditorGUI.GetPropertyHeight(p);
			EditorGUI.PropertyField(rect, p);
			rect.y += rect.height + (rect.height == 0 ? 0 : EditorGUIUtility.standardVerticalSpacing);
		}
		GUI.enabled = prevEnabled;
	}

	public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
	{
		float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		foreach (var p in EnumerateContent(prop))
			height += EditorGUI.GetPropertyHeight(p) + EditorGUIUtility.standardVerticalSpacing;
		height -= EditorGUIUtility.standardVerticalSpacing;
		return height;
	}


	private IEnumerable<SerializedProperty> EnumerateContent(SerializedProperty prop)
	{
		try
		{
			if (prop.objectReferenceValue == null)
				yield break;
		}
		catch (Exception e) { } // unity 2022 introduced "dispose" bug

		var serObj = new SerializedObject(prop.objectReferenceValue);
		var iter   = serObj.GetIterator();
		iter.Next(true);
		foreach (var p in iter.GetOneLevelChildrenProps())
			if (!p.name.StartsWith("m_"))
				yield return p;
		serObj.ApplyModifiedProperties();
	}
}
}