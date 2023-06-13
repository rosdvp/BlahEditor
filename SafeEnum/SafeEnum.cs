using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BlahEditor.SafeEnum
{
[Serializable]
public struct SafeEnum<T> where T : struct, Enum
{
	[SerializeField]
	private string _str;

	private bool _isParsed;
	private bool _isValid;
	private T    _parsedValue;
	
	public SafeEnum(T val)
	{
		_isParsed    = true;
		_isValid     = true;
		_parsedValue = val;
		_str         = val.ToString();
	}
	
	//-----------------------------------------------------------
	//-----------------------------------------------------------
	/// <summary>
	/// False, if value cannot be parsed (i.e. enum value was renamed).<br/>
	/// In this case consider <see cref="BackingStr"/>.
	/// </summary>
	public bool IsValid => _isParsed && _isValid || (!_isParsed && TryParse());
	
	/// <summary>
	/// If <see cref="IsValid"/> false, returns default value.
	/// </summary>
	public T Val
	{
		get
		{
			if (IsValid)
				return _parsedValue;
			Debug.LogWarning($"[blah_editor] parsing failed, backing str: ({_str})");
			return default;
		}
		set
		{
			_isParsed    = true;
			_isValid     = true;
			_parsedValue = value;
			_str         = value.ToString();
		}
	}

	/// <summary>
	/// String with witch value was serialized. You may use it to recover value in case <see cref="IsValid"/> false.
	/// </summary>
	public string BackingStr => _str;

	/// <returns>True if valid.</returns>
	private bool TryParse()
	{
		_isParsed = true;
		_isValid  = Enum.TryParse(_str, out _parsedValue);
		return _isParsed;
	}

	
	public static implicit operator T(SafeEnum<T> a) => a.Val;
	
	public static bool operator ==(SafeEnum<T> a, SafeEnum<T> b)
		=> EqualityComparer<T>.Default.Equals(a.Val, b.Val);
	
	public static bool operator !=(SafeEnum<T> a, SafeEnum<T> b)
		=> !EqualityComparer<T>.Default.Equals(a.Val, b.Val);
	
	public bool Equals(SafeEnum<T> other) => EqualityComparer<T>.Default.Equals(Val, other.Val);
	
	public override bool Equals(object obj) => obj is SafeEnum<T> other && Equals(other);

	public override int GetHashCode() => Val.GetHashCode(); // allocation is impossible

	public override string ToString()
		=> IsValid ? Val.ToString() : $"invalid({_str})";
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SafeEnum<>))]
public class SafeEnumDrawer : PropertyDrawer
{
	public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
	{
		var propStr = prop.FindPropertyRelative("_str");

		var safeEnumType = fieldInfo.FieldType;
		var enumType     = safeEnumType.GenericTypeArguments[0];
		
		var names = new List<string>(enumType.GetEnumNames());

		var isValid = true;
		int  currIdx = names.IndexOf(propStr.stringValue);
		if (currIdx == -1)
		{
			names.Insert(0, $"invalid ({propStr.stringValue})");
			currIdx = 0;
			isValid = false;
		}
		
		var labels = new GUIContent[names.Count];
		for (var i = 0; i < names.Count; i++)
			labels[i] = new GUIContent(names[i]);

		int newIdx = EditorGUI.Popup(rect, label, currIdx, labels);

		if (isValid || newIdx != 0)
			propStr.stringValue = names[newIdx];
	}
}
#endif
}