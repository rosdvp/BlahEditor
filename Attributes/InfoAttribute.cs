using System;
using UnityEngine;

namespace BlahEditor.Attributes
{
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
public class InfoAttribute : PropertyAttribute
{
	public readonly string Text;

	public InfoAttribute(string text)
	{
		Text = text;
	}
}
}