using UnityEditor;

namespace BlahEditor.Editor
{
public static class EditorForceReserializeMenuItem
{
	[MenuItem("Blah/Editor/Force Reserialize Assets")]
	private static void ForceReserialize()
	{
		AssetDatabase.ForceReserializeAssets();
	}
}
}