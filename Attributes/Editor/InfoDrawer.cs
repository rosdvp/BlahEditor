using UnityEditor;
using UnityEngine;

namespace BlahEditor.Attributes.Editor
{
[CustomPropertyDrawer(typeof(InfoAttribute))]
public class InfoDrawer : DecoratorDrawer
{
	private const float SPACE = 5f;
	
	public override void OnGUI(Rect rect)
	{
		string text = GetText();
		rect.height -= SPACE;
		EditorGUI.HelpBox(rect, text, MessageType.Info);
	}

	public override float GetHeight()
		=> GetHelpBoxHeight(GetText()) + SPACE;


	private string GetText() => ((InfoAttribute)attribute).Text;
	
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