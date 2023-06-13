using UnityEngine;

namespace BlahEditor.Attributes
{
public class RefSerializeAttribute : PropertyAttribute
{
	public string FieldEnumName;
	public string MethodCreateName;
	public bool   WithFoldout;

	public RefSerializeAttribute(string fieldEnumName, string methodCreateName, bool withFoldout = true)
	{
		FieldEnumName = fieldEnumName;
		MethodCreateName = methodCreateName;
		WithFoldout      = withFoldout;
	}
}
}