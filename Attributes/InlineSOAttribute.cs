using UnityEngine;

namespace BlahEditor.Attributes
{
public class InlineSOAttribute : PropertyAttribute
{
	public readonly bool IsEditable;

	public InlineSOAttribute(bool isEditable)
	{
		IsEditable = isEditable;
	}
}
}