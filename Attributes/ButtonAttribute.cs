using System;
using UnityEngine;

namespace BlahEditor.Attributes
{
/// <summary>
/// Put this attribute on dummy bool field with attribute <see cref="SerializeField"/>,
/// and specify method name by nameof().
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class ButtonAttribute : PropertyAttribute
{
	public readonly string Text;
	public readonly string MethodName;

	public ButtonAttribute(string text, string methodName)
	{
		Text       = text;
		MethodName = methodName;
	}
}
}