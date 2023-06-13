using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BlahEditor.Attributes.Editor
{
[CustomPropertyDrawer(typeof(TableArrayAttribute))]
public class TableArrayDrawer : PropertyDrawer
{
	private const float FIRST_COLUMN_WIDTH = 20f;
	private const float SPACE_VERT         = 5f;
	private const float SPACE_HORIZ        = 5f;

	private string[] _fieldsNames;
	private float[]  _rowRatio;
	
	public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
	{
		_fieldsNames ??= GetFieldsNames().ToArray();

		if (_rowRatio == null)
		{
			var attr = (TableArrayAttribute)attribute;
			if (attr.RowRatio.Length == 0)
				_rowRatio = BuildRowRatio(_fieldsNames.Length);
			else if (attr.RowRatio.Length == _fieldsNames.Length)
				_rowRatio = attr.RowRatio;
			else
				throw new Exception($"rowRatio length is {attr.RowRatio.Length}, " +
				                    $"but table has {_fieldsNames.Length} columns!");
		}

		int arrayIdx = property.GetIndexInArray();
		if (arrayIdx == 0)
		{
			var headerRect = rect.ToSingleLine();
			EditorGUI.LabelField(headerRect.WithWidth(FIRST_COLUMN_WIDTH), "#");

			var rowsRect = headerRect
			               .ReduceFromLeft(FIRST_COLUMN_WIDTH)
			               .SplitHorizontal(SPACE_HORIZ, _rowRatio);
			for (var i = 0; i < _fieldsNames.Length; i++)
				EditorGUI.LabelField(rowsRect[i], _fieldsNames[i]);

			rect = rect.ReduceFromTop(EditorGUIUtility.singleLineHeight);
		}
		rect.height -= SPACE_VERT;
		
		EditorGUI.LabelField(rect.WithWidth(FIRST_COLUMN_WIDTH), arrayIdx.ToString());
		var rowRects = rect
		               .ReduceFromLeft(FIRST_COLUMN_WIDTH)
		               .SplitHorizontal(SPACE_HORIZ, _rowRatio);
		for (var fieldIdx = 0; fieldIdx < _fieldsNames.Length; fieldIdx++)
		{
			var prop = property.FindPropertyRelative(_fieldsNames[fieldIdx]);

			prop.DrawPropertyFieldWithLabelLength(
				rowRects[fieldIdx],
				GUIContent.none,
				rowRects[fieldIdx].width / 3.0f
			);
		}
	}


	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		float height     = 0f;
		var   enumerator = property.Copy().GetEnumerator();
		while (enumerator.MoveNext())
		{
			var child = enumerator.Current as SerializedProperty;
			height = Math.Max(height, EditorGUI.GetPropertyHeight(child));
		}
		int arrayIdx = property.GetIndexInArray();
		return height + (arrayIdx == 0 ? EditorGUIUtility.singleLineHeight : 0f) + SPACE_VERT;
	}

	private float[] BuildRowRatio(int targetLength)
	{
		float columnRatio = 1f / targetLength;
		var   result      = new float[targetLength];
		for (var i = 0; i < targetLength; i++)
			result[i] = columnRatio;
		return result;
	}

	
	private List<string> GetFieldsNames()
	{
		var names       = new List<string>();
		var elementType = fieldInfo.FieldType.GetElementType();
		var fields = elementType.GetFields(BindingFlags.Instance
		                                   | BindingFlags.Public
		                                   | BindingFlags.NonPublic
		);
		foreach (var field in fields)
			names.Add(field.Name);
		return names;
	}
}
}