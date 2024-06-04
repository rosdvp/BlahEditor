using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;

namespace BlahEditor.Helpers
{
/// <summary>
/// This class is not in Editor folder because it might be used by MonoBeh test methods.
/// </summary>
public static class BlahEditorHelper
{
#if UNITY_EDITOR
	/// <param name="filter">Asset name template.</param>
	/// <param name="result">First found asset.</param>
	/// <param name="isMultipleFound">True, if multiple assets have been found.</param>
	/// <returns>True, if any asset has been found.</returns>
	public static bool TryFindAsset<T>(
		string filter,
		out T result, 
		out bool isMultipleFound) 
		where T : Object
	{
		result          = default;
		isMultipleFound = false;
		
		string[] guids = AssetDatabase.FindAssets($"{filter} t:{typeof(T).Name}");
		if (guids == null || guids.Length == 0)
			return false;
		isMultipleFound = guids.Length > 1;
		string path = AssetDatabase.GUIDToAssetPath(guids[0]);
		return AssetDatabase.LoadAssetAtPath<T>(path);
	}

	/// <param name="neib">Any asset in the searching folder.</param>
	/// <param name="filter">Asset name template.</param>
	/// <returns>All assets in the same folder with <paramref name="neib"/>.</returns>
	public static List<T> FindAssetsInSameFolder<T>(Object neib, string filter) where T: Object
	{
		var result = new List<T>();

		string path = AssetDatabase.GetAssetPath(neib);
		path = string.Join('/', path.Split('/')[..^1]);
		
		string[] guids = AssetDatabase.FindAssets(
			$"{filter} t:{typeof(T).Name}",
			new[] { path }
		);
		foreach (string guid in guids)
		{
			string pathAsset = AssetDatabase.GUIDToAssetPath(guid);
			var    asset     = AssetDatabase.LoadAssetAtPath<T>(pathAsset);
			if (asset != null)
				result.Add(asset);
		}
		return result;
	}
#endif
}
}