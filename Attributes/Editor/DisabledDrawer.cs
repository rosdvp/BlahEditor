using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BlahEditor.Attributes.Editor
{
[CustomPropertyDrawer(typeof(DisabledAttribute))]
[CustomPropertyDrawer(typeof(EnabledIfAttribute))]
[CustomPropertyDrawer(typeof(DisabledIfAttribute))]
public class DisabledDrawer : PropertyDrawer
{
	private bool _isSetup;

	private bool         _isWithPredicate;
	private FieldInfo    _memberField;
	private PropertyInfo _memberProp;
	private bool         _memberTargetValue;

	public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
	{
		TrySetup(property);

		bool isEnabled = _isWithPredicate && GetMemberValue(property) == _memberTargetValue;
		isEnabled &= GUI.enabled;
		
		if (!isEnabled)
			GUI.enabled = false;
		EditorGUI.PropertyField(rect, property, label);
		if (!isEnabled)
			GUI.enabled = true;
	}


	private void TrySetup(SerializedProperty property)
	{
		if (_isSetup)
			return;
		if (attribute is DisabledAttribute)
		{
			_isSetup         = true;
			_isWithPredicate = false;
			return;
		}
		_isWithPredicate = true;

		string memberName = null;
		if (attribute is EnabledIfAttribute enabledIfAttr)
		{
			memberName         = enabledIfAttr.MemberName;
			_memberTargetValue = true;
		}
		else if (attribute is DisabledIfAttribute disabledIfAttr)
		{
			memberName         = disabledIfAttr.MemberName;
			_memberTargetValue = false;
		}
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
		object holderObject = property.GetHolderObject();
		if (_memberField?.GetValue(holderObject) is bool fieldValue)
			return fieldValue;
		if (_memberProp?.GetValue(holderObject) is bool propValue)
			return propValue;
		throw new Exception($"Invalid member value at Attribute for {property.displayName}");
	}
}
}