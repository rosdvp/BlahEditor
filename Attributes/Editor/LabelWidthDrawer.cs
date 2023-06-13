using System;
using UnityEditor;
using UnityEngine;

namespace BlahEditor.Attributes.Editor
{
[CustomPropertyDrawer(typeof(LabelWidthAttribute))]
public class LabelWidthDrawer : PropertyDrawer
{
	public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
	{
		var attr = (LabelWidthAttribute)attribute;

		float old = EditorGUIUtility.labelWidth;
		EditorGUIUtility.labelWidth = attr.Width;
		EditorGUI.PropertyField(pos, prop, label);
		EditorGUIUtility.labelWidth = old;
	}
}
}