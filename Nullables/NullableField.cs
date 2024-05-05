using System;
using BlahEditor.DrawersExtensions;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BlahEditor.Nullables
{
[Serializable]
public struct NullableField<T>
{
	[SerializeField]
	private bool _hasValue;
	[SerializeField]
	private T _value;

	public T Value
	{
		get => _hasValue ? _value : throw new Exception("_hasValue is false");
		set => _value = value;
	}

	public bool HasValue => _hasValue;
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(NullableField<>))]
public class EditorNullableFloatDrawer : PropertyDrawer
{
	public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
	{
		var propHasValue = prop.FindPropertyRelative("_hasValue");
		var propValue    = prop.FindPropertyRelative("_value");

		var rects = rect.SplitHorizontal(5, 0.4f, 0.05f, 0.55f);

		EditorGUI.LabelField(rects[0], label);
		EditorGUI.PropertyField(rects[1], propHasValue, GUIContent.none);

		if (propHasValue.boolValue)
			EditorGUI.PropertyField(rects[2], propValue, GUIContent.none);
	}
}
#endif
}