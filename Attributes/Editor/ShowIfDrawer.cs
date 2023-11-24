using System;
using System.Reflection;
using BlahEditor.DrawersExtensions;
using UnityEditor;
using UnityEngine;

namespace BlahEditor.Attributes.Editor
{
[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfDrawer : PropertyDrawer
{
	private bool         _isSetup;
	private FieldInfo    _memberField;
	private PropertyInfo _memberProp;

	public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
	{
		if (GetMemberValue(property))
			EditorGUI.PropertyField(rect, property, label);
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return GetMemberValue(property) ? base.GetPropertyHeight(property, label) : 0f;
	}
	
	
	private void TrySetup(SerializedProperty property)
	{
		if (_isSetup)
			return;

		string memberName = ((ShowIfAttribute)attribute).MemberName;

		object holderObject = property.GetHolderObject();
		var    holderType   = holderObject.GetType();
		var member = holderType.GetMember(memberName, BindingFlags.Instance |
		                                              BindingFlags.Public |
		                                              BindingFlags.NonPublic)[0];
		
		if (member is FieldInfo memberField)
			_memberField = memberField;
		else if (member is PropertyInfo memberProp)
			_memberProp = memberProp;
		_isSetup = true;
	}

	private bool GetMemberValue(SerializedProperty property)
	{
		TrySetup(property);
		
		object holderObject = property.GetHolderObject();
		if (_memberField?.GetValue(holderObject) is bool fieldValue)
			return fieldValue;
		if (_memberProp?.GetValue(holderObject) is bool propValue)
			return propValue;
		throw new Exception($"Invalid member value at Attribute for {property.displayName}");
	}
}
}