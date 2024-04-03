using UnityEditor;
using UnityEngine;

namespace BlahEditor.Attributes.Editor
{
[CustomPropertyDrawer(typeof(InfoAttribute))]
public class InfoDrawer : DecoratorDrawer
{
	private const float SPACE = 5f;

	private float _height;
	
	public override void OnGUI(Rect rect)
	{
		string text = ((InfoAttribute)attribute).Text;
		rect.height -= SPACE;
		EditorGUI.HelpBox(rect, text, MessageType.Info);

		_height = GetHelpBoxHeight(text) + SPACE;
	}

	public override float GetHeight() => _height;
	
	private float GetHelpBoxHeight(string text)
	{
		float minHeight = EditorGUIUtility.singleLineHeight;
		float height = GUI.skin.box.CalcHeight(new GUIContent(text),
		                                       EditorGUIUtility.currentViewWidth)
		               + 4;
		return Mathf.Max(minHeight, height);
	}
}
}