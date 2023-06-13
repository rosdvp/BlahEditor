using System;
using UnityEngine;

namespace BlahEditor.Attributes
{
/// <summary>
/// This field will be non-editable.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
public class DisabledAttribute : PropertyAttribute
{
}

/// <summary>
/// This field will be editable only if specified member (field or property) is TRUE.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
public class EnabledIfAttribute : PropertyAttribute
{
	public string MemberName;

	public EnabledIfAttribute(string memberName) 
		=> MemberName  = memberName;
}

/// <summary>
/// This field will be editable only if specified member (field or property) is FALSE.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
public class DisabledIfAttribute : PropertyAttribute
{
	public string MemberName;

	public DisabledIfAttribute(string memberName)
		=> MemberName = memberName;
}
}