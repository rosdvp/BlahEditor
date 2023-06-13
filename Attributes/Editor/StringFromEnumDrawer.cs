using System;
using UnityEditor;
using UnityEngine;

namespace BlahEditor.Attributes.Editor
{
[CustomPropertyDrawer(typeof(StringFromEnumAttribute))]
public class StringFromEnumDrawer : PropertyDrawer
{
	public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
	{
		var enumType = ((StringFromEnumAttribute)attribute).EnumType;

		string[] names = Enum.GetNames(enumType);

		if (Enum.TryParse(enumType, prop.stringValue, out object rawVal))
		{
			var val = (Enum)rawVal;
			val              = EditorGUI.EnumPopup(pos, label, val);
			prop.stringValue = val.ToString();
		}
		else
		{
			Array.Resize(ref names, names.Length+1);
			names[^1] = $"invalid ({prop.stringValue})";

			int idx = EditorGUI.Popup(pos, label.text, names.Length - 1, names);
			if (idx < names.Length - 1)
			{
				prop.stringValue = Enum.ToObject(enumType, idx).ToString();
			}
		}
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return base.GetPropertyHeight(property, label);
	}
}
}