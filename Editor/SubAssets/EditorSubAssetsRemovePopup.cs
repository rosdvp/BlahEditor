using UnityEditor;

namespace BlahEditor.Editor.SubAssets
{
public static class EditorSubAssetsRemovePopup
{
	[MenuItem("Assets/Blah/Remove selected SubAssets")]
	public static void RemoveSelected()
	{
		foreach (var selected in Selection.objects)
			if (AssetDatabase.IsSubAsset(selected))
				AssetDatabase.RemoveObjectFromAsset(selected);
		AssetDatabase.SaveAssets();
	}
}
}