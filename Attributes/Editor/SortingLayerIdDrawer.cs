using System;
using UnityEditor;
using UnityEngine;

namespace BlahEditor.Attributes.Editor
{
[CustomPropertyDrawer(typeof(SortingLayerIdAttribute))]
public class SortingLayerIdDrawer : PropertyDrawer
{
	private SortingLayer[] _all;
	private string[]       _names;
	
	public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
	{
		if (prop.propertyType != SerializedPropertyType.Integer)
		{
			EditorGUI.LabelField(rect, label.text, "Int field required");
			return;
		}

		if (_all == null)
		{
			_all   = SortingLayer.layers;
			_names = new string[_all.Length];
			for (int i = 0; i < _all.Length; i++)
				_names[i] = _all[i].name;
		}

		int idx = Array.FindIndex(_all, s => s.id == prop.intValue);
		if (idx < 0 || idx >= _all.Length)
			idx = 0;

		idx = EditorGUI.Popup(rect, label.text, idx, _names);

		prop.intValue = _all[idx].id;
	}
}
}