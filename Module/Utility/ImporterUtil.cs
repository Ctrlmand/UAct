using UnityEditor;

namespace UAct.Util
{
	public static class ImporterUtil
	{
		public static void CleanNullExternalAssets(ModelImporter importer)
		{
			string path = AssetDatabase.GetAssetPath(importer);
			var map = importer.GetExternalObjectMap();
			foreach (var item in map)
			{
				if (item.Value == null)
				{
					importer.RemoveRemap(item.Key);
				}
			}
			AssetDatabase.WriteImportSettingsIfDirty(path);
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
			return;
		}

	}
}