using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BlahEditor.Attributes.Editor
{
public static class SerializedPropertyExtensions
{
	/// <summary>
	/// Use this method when you want to access another member of object.<br/>
	/// For example, if you want to invoke a method.
	/// </summary>
	/// <returns>
	///	Object in which <paramref name="prop"/> is stored.<br/>
	/// For example, if <paramref name="prop"/> is a field inside Scriptable Object,
	/// method will return this instance of Scriptable Object.<br/>
	/// </returns>
	public static object GetHolderObject(this SerializedProperty prop)
	{
		object obj  = prop.serializedObject.targetObject;
		string path = prop.propertyPath.Replace("Array.data[", "[");
		foreach(string fieldName in path.Split('.')[..^1])
		{
			if (fieldName[0] == '[')
				obj = GetObjectValueInArray(obj, int.Parse(fieldName[1..^1]));
			else
			{
				var type = obj.GetType();
				var field = type.GetField(fieldName,
				                          BindingFlags.Instance
				                          | BindingFlags.Public
				                          | BindingFlags.NonPublic);
				obj = field.GetValue(obj);
			}
		}
		return obj;
	}

	private static object GetObjectValueInArray(object holder, int index)
	{
		var enumerable = (IEnumerable)holder;
		if (enumerable == null) 
			return null;
		var iterator = enumerable.GetEnumerator();
		for (var i = 0; i <= index; i++)
			if (!iterator.MoveNext())
				return null;
		return iterator.Current;
	}

	//-----------------------------------------------------------
	//-----------------------------------------------------------
	/// <param name="prop">Must be an element in array.</param>
	/// <returns>
	/// Index of <see cref="prop"/> inside array.
	/// </returns>
	public static int GetIndexInArray(this SerializedProperty prop)
	{
		int startIdx = prop.propertyPath.LastIndexOf('[') + 1;
		int endIdx   = prop.propertyPath.LastIndexOf(']');
		return int.Parse(prop.propertyPath[startIdx..endIdx]);
	}

	//-----------------------------------------------------------
	//-----------------------------------------------------------
	/// <returns>
	///	Property of array that contains <see cref="prop"/>.
	/// </returns>
	public static SerializedProperty GetArrayFromArrayElement(this SerializedProperty prop)
	{
		string[] pathItems  = prop.propertyPath.Split('.');
		string   parentPath = string.Join(".", pathItems[..^1]);
		return prop.serializedObject.FindProperty(parentPath);
	}

	//-----------------------------------------------------------
	//-----------------------------------------------------------
	/// <returns>
	///	All serialized properties that are inside <paramref name="prop"/>.
	/// </returns>
	public static IEnumerable<SerializedProperty> GetOneLevelChildrenProps(this SerializedProperty prop)
	{
		prop = prop.Copy();
		var propAfterLast = prop.Copy();
		propAfterLast.Next(false);

		if (!prop.Next(true))
			yield break;
		do
		{
			yield return prop;
		} while (prop.Next(false) && !SerializedProperty.EqualContents(prop, propAfterLast));
	}

	//-----------------------------------------------------------
	//-----------------------------------------------------------
	public static void DrawPropertyFieldWithLabelLength(
		this SerializedProperty prop,
		Rect                    propRect,
		GUIContent              label,
		float                   labelWidth)
	{
		float old = EditorGUIUtility.labelWidth;
		EditorGUIUtility.labelWidth = labelWidth;
		EditorGUI.PropertyField(propRect, prop, label);
		EditorGUIUtility.labelWidth = old;
	}
}
}