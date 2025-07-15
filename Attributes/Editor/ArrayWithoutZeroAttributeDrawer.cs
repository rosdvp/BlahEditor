using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BlahEditor.Attributes.Editor
{
[CustomPropertyDrawer(typeof(ArrayWithoutZeroAttribute))]
public class ArrayWithoutZeroAttributeDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
	{
		int idx = GetIdxInArray(prop);
		return idx == 0 ? EditorGUIUtility.singleLineHeight : EditorGUI.GetPropertyHeight(prop, label);
	}

	public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
	{
		EditorGUI.BeginProperty(rect, label, prop);
		int idx = GetIdxInArray(prop);
		if (idx == 0)
			EditorGUI.LabelField(rect, "Array starts from 1");
		else
			EditorGUI.PropertyField(rect, prop, label, true);
		EditorGUI.EndProperty();
	}

	private int GetIdxInArray(SerializedProperty prop)
	{
		//path is arr.items[x]
		string path = prop.propertyPath;
		int    idx  = int.Parse(path.Split('[').LastOrDefault().TrimEnd(']'));
		return idx;
	}
}
}