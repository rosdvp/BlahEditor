using System.Collections.Generic;
using UnityEditor;

namespace BlahEditor.Editor
{
public static class EditorForceReserializeMenu
{
	[MenuItem("Assets/Blah/Force Reserialize")]
	private static void ForceReserialize()
	{
		var paths = new List<string>();
		foreach (var obj in Selection.objects)
			paths.Add(AssetDatabase.GetAssetPath(obj));
		AssetDatabase.ForceReserializeAssets(paths);
	}
}
}