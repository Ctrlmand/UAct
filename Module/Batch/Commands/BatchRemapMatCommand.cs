using UnityEngine;
using UnityEditor;
using System.IO;

namespace UAct.Batch
{
	using Util;

	public class BatchRemapMatCommand : ICommand
	{
		string matDir = null;
		public void Execute(ICommandContext context)
		{
			matDir = context.GetData<string>();
			Debug.Log(matDir);
			if (matDir == null)
			{
				SearchRemapMaterials();
			}
			else
			{
				SearchRemapInTarget();
			}

		}

		private void SearchRemapMaterials()
		{
			foreach (Object obj in Selection.objects)
			{
				string objPath = AssetDatabase.GetAssetPath(obj);
				ModelImporter importer = AssetImporter.GetAtPath(objPath) as ModelImporter;
				if (importer == null) continue;

				// Clear null and Remap
				ImporterUtil.CleanNullExternalAssets(importer);
				importer.SearchAndRemapMaterials(ModelImporterMaterialName.BasedOnMaterialName, ModelImporterMaterialSearch.RecursiveUp);
				// Apply
				string assetPath = AssetDatabase.GetAssetPath(obj);
				AssetDatabase.WriteImportSettingsIfDirty(assetPath);
				// AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
			}
			AssetDatabase.Refresh();
		}

		private void SearchRemapInTarget()
		{
			Debug.Log(matDir);
			string[] matGUIDs = AssetDatabase.FindAssets("t:Material", new string[] { matDir });

			foreach (var item in Selection.objects)
			{
				string itemPath = AssetDatabase.GetAssetPath(item);
				ModelImporter importer = AssetImporter.GetAtPath(itemPath) as ModelImporter;
				if (importer == null) continue;

				// Clear null
				ImporterUtil.CleanNullExternalAssets(importer);

				// Get all embedded materials and remap
				foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(itemPath))
				{
					// Find material
					for (int i = 0; i < matGUIDs.Length; i++)
					{
						string matPath = AssetDatabase.GUIDToAssetPath(matGUIDs[i]);
						Debug.Log(matPath);
						if (asset.name == Path.GetFileNameWithoutExtension(matPath))
						{
							// match success
							importer.AddRemap(new AssetImporter.SourceAssetIdentifier(asset), AssetDatabase.LoadAssetAtPath<Material>(matPath));
							break;
						}
					}
				}
				// Apply
				string assetPath = AssetDatabase.GetAssetPath(item);
				AssetDatabase.WriteImportSettingsIfDirty(assetPath);
				// AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
			}
			AssetDatabase.Refresh();

		}

	}
}
