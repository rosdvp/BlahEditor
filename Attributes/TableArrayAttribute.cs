using System;
using UnityEngine;

namespace BlahEditor.Attributes
{
/// <summary>
/// Put the attribute on array of classes field.<br/>
/// Elements will be drawn vertically, but each field of element will be drawn horizontally.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class TableArrayAttribute : PropertyAttribute
{
	public readonly float[] RowRatio;

	public TableArrayAttribute() 
		=> RowRatio = Array.Empty<float>();

	public TableArrayAttribute(params float[] rowRatio) 
		=> RowRatio = rowRatio;
}
}