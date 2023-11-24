using UnityEditor;
using UnityEngine;

namespace BlahEditor.DrawersExtensions
{
public static class RectExtensions
{
	public static Rect ToSingleLine(this Rect rect) 
		=> new(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);

	public static Rect ToNextLine(this Rect rect)
		=> new(rect.x,
		       rect.y + rect.height + EditorGUIUtility.standardVerticalSpacing,
		       rect.width,
		       rect.height
		);

	public static Rect WithWidth(this Rect rect, float width) 
		=> new(rect.x, rect.y, width, rect.height);

	public static Rect WithHeight(this Rect rect, float height)
		=> new(rect.x, rect.y, rect.width, height);

	public static Rect ReduceFromLeft(this Rect rect, float offset)
		=> new(rect.x + offset, rect.y, rect.width - offset, rect.height);

	public static Rect ReduceFromTop(this Rect rect, float offset)
		=> new(rect.x, rect.y + offset, rect.width, rect.height - offset);

	public static Rect[] SplitHorizontal(this Rect rect, float space, params float[] ratio)
	{
		var   result = new Rect[ratio.Length];
		float posX   = rect.x;
		for (var i = 0; i < ratio.Length; i++)
		{
			if (i != 0)
				posX += space;
			float width = rect.width * ratio[i] - space;
			result[i] =  new Rect(posX, rect.y, width, rect.height);
			posX      += width;
		}
		return result;
	}
}
}