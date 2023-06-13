using System;
using UnityEngine;

namespace BlahEditor.Attributes
{
public class ArrayByEnumAttribute : PropertyAttribute
{
    public readonly string[] Names;

    public ArrayByEnumAttribute(Type enumType) 
        => Names = Enum.GetNames(enumType);
}
}