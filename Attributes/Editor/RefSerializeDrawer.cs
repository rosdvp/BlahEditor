using System;
using System.Reflection;
using BlahEditor.DrawersExtensions;
using UnityEditor;
using UnityEngine;

namespace BlahEditor.Attributes.Editor
{
[CustomPropertyDrawer(typeof(RefSerializeAttribute))]
public class RefSerializeDrawer : PropertyDrawer
{
	public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
	{
		var attr = (RefSerializeAttribute)attribute;

		prop.managedReferenceValue ??= CreateObject(attr.MethodCreateName, 0);
		
		pos = pos.ToSingleLine();

		if (attr.WithFoldout)
		{
			prop.isExpanded = EditorGUI.Foldout(pos, prop.isExpanded, label);
			if (!prop.isExpanded)
				return;
			pos = pos.ToNextLine();
		}

		if (attr.WithFoldout)
			EditorGUI.indentLevel += 1;
		
		var propEnum     = prop.FindPropertyRelative(attr.FieldEnumName);
		int oldEnumValue = propEnum.enumValueIndex;
		EditorGUI.PropertyField(pos, propEnum);
		int newEnumValue = propEnum.enumValueIndex;

		if (newEnumValue != oldEnumValue)
		{
			prop.managedReferenceValue = CreateObject(attr.MethodCreateName, newEnumValue);
			propEnum = prop.FindPropertyRelative(attr.FieldEnumName);
			propEnum.enumValueIndex = newEnumValue;
		}

		foreach (var child in prop.GetOneLevelChildrenProps())
		{
			if (child.propertyPath == propEnum.propertyPath)
				continue;
			pos = pos.ToNextLine();
			EditorGUI.PropertyField(pos, child);
		}
		
		if (attr.WithFoldout)
			EditorGUI.indentLevel -= 1;
	}

	public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
	{
		var attr = (RefSerializeAttribute)attribute;

		if (attr.WithFoldout && !prop.isExpanded)
			return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

		float height = attr.WithFoldout
			? EditorGUIUtility.singleLineHeight +
			  EditorGUIUtility.standardVerticalSpacing
			: 0f;

		foreach (var child in prop.GetOneLevelChildrenProps())
		{
			height += EditorGUI.GetPropertyHeight(child);
			height += EditorGUIUtility.standardVerticalSpacing;	
		}
		return height;
	}

	private object CreateObject(string methodName, int enumValue)
	{
		var type = fieldInfo.FieldType;
		if (type.IsArray)
			type = type.GetElementType();
		
		var method = type.GetMethod(methodName,
		                            BindingFlags.Static |
		                            BindingFlags.Public |
		                            BindingFlags.NonPublic
		);
		if (method == null)
			throw new Exception($"Failed to find method {methodName} in type {type}");
		
		return method?.Invoke(null, new object[] { enumValue });
	}
}
}