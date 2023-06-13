using System;
using UnityEngine;

namespace BlahEditor.Attributes
{
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
public class LabelWidthAttribute : PropertyAttribute
{
	public float Width;

	public LabelWidthAttribute(float width)
	{
		Width = width;
	}
}
}