using System;
using System.Collections.Generic;
using BlahEditor.Attributes.Editor;
using BlahEditor.DrawersExtensions;
using UnityEditor;
using UnityEngine;

namespace BlahEditor.EnumDict
{
/// <summary>
/// Must be used only in ScriptableObject classes for read-only purpose.
/// </summary>
[Serializable]
public class EnumDict<TKey, TValue> : Dictionary<TKey, TValue> where TKey : Enum
{
	[SerializeField]
	private TKey[] _keys;
	[SerializeField]
	private TValue[] _values;
	
	//-----------------------------------------------------------
	//-----------------------------------------------------------
	public void OnBeforeSerialize() { }

	public void OnAfterDeserialize()
	{
		this.Clear();
		for (var i = 0; i < _keys.Length; i++)
			this[_keys[i]] = _values[i];
	}
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(EnumDict<,>))]
public class SerDictDrawer : PropertyDrawer
{
	private bool _isFoldout = false;

	private NoFoldoutDrawer _itemDrawer = new();
	
	public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
	{
		rect       = rect.ToSingleLine();
		_isFoldout = EditorGUI.Foldout(rect, _isFoldout, label);
		if (!_isFoldout)
			return;
		
		var propKeys   = prop.FindPropertyRelative("_keys");
		var propValues = prop.FindPropertyRelative("_values");

		if (propKeys.arraySize != propValues.arraySize)
			propValues.arraySize = propKeys.arraySize;

		int count = propKeys.arraySize;
		if (count > 0)
		{
			float height = Math.Max(
				EditorGUI.GetPropertyHeight(propKeys.GetArrayElementAtIndex(0), GUIContent.none),
				_itemDrawer.GetPropertyHeight(propValues.GetArrayElementAtIndex(0), GUIContent.none)
			);
			
			var seenKeys = new HashSet<int>();
			
			for (var i = 0; i < count; i++)
			{
				rect = rect.ToNextLine().WithHeight(height);
				var rectsItem = rect.SplitHorizontal(5, 0.05f, 0.425f, 0.425f, 0.1f);

				int key = propKeys.GetArrayElementAtIndex(i).enumValueIndex;
				if (seenKeys.Contains(key))
					EditorGUI.HelpBox(rectsItem[0], null, MessageType.Error);
				seenKeys.Add(key);

				EditorGUI.PropertyField(rectsItem[1],
				                        propKeys.GetArrayElementAtIndex(i),
				                        GUIContent.none
				);
				
				EditorGUIUtility.labelWidth /= 2.5f;
				_itemDrawer.OnGUI(rectsItem[2],
				                  propValues.GetArrayElementAtIndex(i),
				                  GUIContent.none
				);
				EditorGUIUtility.labelWidth *= 2.5f;
				
				if (GUI.Button(rectsItem[3], "-"))
				{
					propKeys.DeleteArrayElementAtIndex(i);
					propValues.DeleteArrayElementAtIndex(i);
					return;
				}
			}
		}

		rect = rect.ToNextLine().ToSingleLine();
		if (GUI.Button(rect, "+"))
		{
			propKeys.InsertArrayElementAtIndex(propKeys.arraySize);
			propValues.InsertArrayElementAtIndex(propValues.arraySize);
		}
	}

	public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
	{
		float height = 0;
		float space  = EditorGUIUtility.standardVerticalSpacing;
		
		height += EditorGUIUtility.singleLineHeight; // label
		height += space;
		
		var propKeys   = prop.FindPropertyRelative("_keys");
		var propValues = prop.FindPropertyRelative("_values");
		if (_isFoldout)
		{
			if (propKeys.arraySize > 0)
			{
				float itemHeight = Math.Max(
					EditorGUI.GetPropertyHeight(propKeys.GetArrayElementAtIndex(0), GUIContent.none),
					_itemDrawer.GetPropertyHeight(propValues.GetArrayElementAtIndex(0), GUIContent.none)
				);
				height += (itemHeight + space) * propKeys.arraySize;
			}

			height += EditorGUIUtility.singleLineHeight; // add button
			height += space;
		}
		return height;
	}
}
#endif
}