using System;
using UnityEngine;

namespace BlahEditor.Attributes
{
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class ArrayWithoutZeroAttribute : PropertyAttribute { }
}