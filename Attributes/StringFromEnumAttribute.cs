using System;
using UnityEngine;

namespace BlahEditor.Attributes
{
/// <summary>
/// Use this attribute on string SerializedField to turn enum value into string.<br/>
/// This is useful when you change order of enum values or inserting new ones.<br/>
/// However, if you change name of enum value, you must re-assign this field in inspector,
/// since the string will become invalid.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class StringFromEnumAttribute : PropertyAttribute
{
	public readonly Type EnumType;

	public StringFromEnumAttribute(Type enumType) => EnumType = enumType;
}
}