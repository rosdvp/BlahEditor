using System;
using UnityEngine;

namespace BlahEditor.Attributes
{
/// <summary>
/// This field will be shown only if specified member (field or property) is TRUE.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
public class ShowIfAttribute : PropertyAttribute
{
	public readonly string MemberName;

	public ShowIfAttribute(string memberName)
	{
		MemberName  = memberName;
	}
}
}