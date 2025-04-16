using System.IO;
using UnityEditor;
using UnityEngine;

namespace BlahEditor.Editor
{
public static class EditorBatchDuplicate
{
	[MenuItem("Assets/Blah/Batch Duplicate")]
	public static void DuplicateSelected()
	{
		string oldFolderGuid = Selection.assetGUIDs[0];
		string oldFolderPath = AssetDatabase.GUIDToAssetPath(oldFolderGuid);

		string newFolderPath = oldFolderPath + "_duplicate";

		AssetDatabase.CopyAsset(oldFolderPath, newFolderPath);

		string[] newGuids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { newFolderPath });

		foreach (string guid in newGuids)
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			var    so   = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);

			using var ser  = new SerializedObject(so);
			var       iter = ser.GetIterator();
			iter.NextVisible(true);  // skip Base
			iter.NextVisible(false); // skip m_Script
			do
			{
				if (iter.propertyType != SerializedPropertyType.ObjectReference)
					continue;

				string oldPath = AssetDatabase.GetAssetPath(iter.objectReferenceValue);
				string newPath = newFolderPath + oldPath[oldFolderPath.Length..];

				if (File.Exists(newPath))
					iter.objectReferenceValue = AssetDatabase.LoadAssetAtPath<Object>(newPath);
			}
			while (iter.NextVisible(true));

			ser.ApplyModifiedPropertiesWithoutUndo();
		}

		AssetDatabase.SaveAssets();
	}
}
}