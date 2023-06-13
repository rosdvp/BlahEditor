#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BlahEditor.Helpers
{
/// <summary>
/// This class is not in Editor folder because it might be used by MonoBeh test methods.
/// </summary>
public static class EditorHelpers
{
	/// <param name="nameFilter">If null, name is not used for filtering.</param>
	/// <returns>
	/// First asset in Game folder that satisfies <typeparamref name="T"/> and <paramref name="nameFilter"/>
	/// </returns>
	public static T FindScriptableObjectInstance<T>(string nameFilter = null) where T : ScriptableObject
	{
		string[] guids = AssetDatabase.FindAssets($"{nameFilter} t:{typeof(T).Name}");
		if (guids == null || guids.Length == 0)
			throw new Exception($"SO {nameof(T)} with filter ({nameFilter}) is not found");

		string path = AssetDatabase.GUIDToAssetPath(guids[0]);
		return AssetDatabase.LoadAssetAtPath<T>(path);
	}

	public static T FindAsset<T>(string nameFilter = null) where T: Object
	{
		string[] guids = AssetDatabase.FindAssets($"{nameFilter} t:{typeof(T).Name}");
		if (guids == null || guids.Length == 0)
			throw new Exception($"Asset {typeof(T).Name} with filter ({nameFilter}) is not found");
		if (guids.Length > 1)
		{
			var errorStr = "";
			foreach (string guid in guids)
				errorStr += AssetDatabase.GUIDToAssetPath(guids[0]) + "\n";
			
			Debug.LogError(errorStr);
			throw new Exception($"Multiple assets {typeof(T).Name} with filter ({nameFilter}) exist");
		}

		string path = AssetDatabase.GUIDToAssetPath(guids[0]);
		return AssetDatabase.LoadAssetAtPath<T>(path);
	}
}
}
#endif