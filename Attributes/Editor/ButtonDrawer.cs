using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BlahEditor.Attributes.Editor
{
[CustomPropertyDrawer(typeof(ButtonAttribute))]
public class ButtonDrawer : PropertyDrawer
{
	private const float SPACE = 5f;

	public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
	{
		string text       = GetText();
		string methodName = ((ButtonAttribute)attribute).MethodName;

		object target = property.GetHolderObject();
		var    method = (MethodInfo)property.FindNeighborMember(methodName);
		if (method == null)
			throw new Exception($"Failed to find method {methodName}");
		if (method.GetParameters().Length > 0)
			throw new Exception($"Method {method} must not have any parameters");

		rect.y      += SPACE;
		rect.height -= SPACE;

		if (GUI.Button(rect, text))
			method.Invoke(target, null);
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) 
		=> GetButtonHeight(GetText()) + SPACE * 2f;
	
	private string GetText() => ((ButtonAttribute)attribute).Text;

	private float GetButtonHeight(string text)
		=> GUI.skin.button.CalcHeight(new GUIContent(text), EditorGUIUtility.currentViewWidth);
}
}