using BlahEditor.DrawersExtensions;
using UnityEditor;
using UnityEngine;

namespace BlahEditor.Attributes.Editor
{
[CustomPropertyDrawer(typeof(NoFoldoutAttribute))]
public class NoFoldoutDrawer : PropertyDrawer
{
	private const float SPACE = 2f;

	public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
	{
		if (property.propertyType != SerializedPropertyType.Generic)
		{
			EditorGUI.PropertyField(rect, property, label);
			return;
		}
		
		foreach (var prop in property.GetOneLevelChildrenProps())
		{
			rect.height = EditorGUI.GetPropertyHeight(prop);
			EditorGUI.PropertyField(rect, prop, true);
			rect.y += rect.height + (rect.height == 0 ? 0 : SPACE);
		}
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		if (property.propertyType != SerializedPropertyType.Generic)
			return EditorGUI.GetPropertyHeight(property, label);
		
		var height     = 0f;
		foreach (var prop in property.GetOneLevelChildrenProps())
		{
			float propHeight = EditorGUI.GetPropertyHeight(prop);
			height += propHeight + (propHeight == 0 ? 0 : SPACE);
		}
		return height;
	}
}
}