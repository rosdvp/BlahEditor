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
		if (!string.IsNullOrEmpty(label.text))
		{
			EditorGUI.PropertyField(rect, prop, label);
			rect = rect.ToNextLine();
		}

		EditorGUI.BeginDisabledGroup(!isEditable);
		rect = rect.ReduceFromLeft(10);
		foreach (var p in EnumerateContent(prop))
		{
			rect.height = EditorGUI.GetPropertyHeight(p, true);
			EditorGUI.PropertyField(rect, p, true);
			rect.y += rect.height + (rect.height == 0 ? 0 : EditorGUIUtility.standardVerticalSpacing);
		}
		EditorGUI.EndDisabledGroup();
	}

	public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
	{
		float height = 0;
		if (!string.IsNullOrEmpty(label.text))
			height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		foreach (var p in EnumerateContent(prop))
			height += EditorGUI.GetPropertyHeight(p, true) + 
			          EditorGUIUtility.standardVerticalSpacing;
		height -= EditorGUIUtility.standardVerticalSpacing;
		return height;
	}


	private IEnumerable<SerializedProperty> EnumerateContent(SerializedProperty prop)
	{
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