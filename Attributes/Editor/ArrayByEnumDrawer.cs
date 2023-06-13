using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BlahEditor.Attributes.Editor
{
[CustomPropertyDrawer(typeof(ArrayByEnumAttribute))]
public class ArrayByEnumAttributeDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label);
    }

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(rect, label, property);
        try
        {
            string[] names = ((ArrayByEnumAttribute)attribute).Names;

            //path is arr.items[x]
            string path    = property.propertyPath;
            string arrPath = path[..path.LastIndexOf('.')];
            var    arrProp = property.serializedObject.FindProperty(arrPath);

            if (arrProp.arraySize < names.Length)
                arrProp.arraySize = names.Length;

            int idx = int.Parse(path.Split('[').LastOrDefault().TrimEnd(']'));
            EditorGUI.PropertyField(rect, property,
                                    new GUIContent(
                                        ObjectNames.NicifyVariableName(names[idx])),
                                    true);
        }
        catch
        {
            EditorGUI.PropertyField(rect, property, new GUIContent("none"), true);
        }
        EditorGUI.EndProperty();
    }
}
}