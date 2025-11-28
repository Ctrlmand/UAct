using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using System.IO;

namespace UAct.AssetsProcessing
{
	using Util;

    public class ExtractMaterialCommand : ICommand
    {
        public void Execute(ICommandContext context)
        {
            // find shader
            Shader shader = context.GetData<Shader>();
            context.ShowAll();

            shader = shader ?? GetPipelineShader();
            if (shader == null)
            {
                Debug.LogWarning("Can't find shader");
                return;
            }
            
            foreach (GameObject obj in Selection.gameObjects)
            {
                // Create folder
                string targetFolder = AssetMethod.CreateMatFolderForObj(obj, "Materials");
                // Get model importer and clean null external assets
                ModelImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(obj)) as ModelImporter;
                if (importer != null) ImporterUtil.CleanNullExternalAssets(importer);

                // Get embedded materials
                List<Material> embeddedMaterials = GetEmbeddedMaterials(obj);
                if (embeddedMaterials.Count < 1)
                {
                    Debug.LogWarning("Model has no embedded material");
                    return;
                }

                // Set materials
                foreach (Material item in embeddedMaterials)
                {
                    // Debug.Log(mat.name);
                    Material mat = CreateMaterial(targetFolder, item.name, shader);
                    importer.AddRemap(new AssetImporter.SourceAssetIdentifier(item), mat);

                }
                // Write changes to importer and update
                string assetPath = AssetDatabase.GetAssetPath(obj);
                AssetDatabase.WriteImportSettingsIfDirty(assetPath);
                // AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            }

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            return;
        }
        
        private List<Material> GetEmbeddedMaterials(GameObject obj)
        {
            List<Material> embeddedMaterials = new List<Material>();
            foreach (var item in AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(obj)))
            {
                if (item is Material mat)
                {
                    embeddedMaterials.Add(mat);
                }
            }
            return embeddedMaterials;
        }

        private Material CreateMaterial(string folderPath, string matName, Shader shader)
        {
            Material mat = new Material(shader);
            string matPath = $"{folderPath}/{matName}.mat";

            // Mat is Exist
            if (!File.Exists(matPath))
            {
                AssetDatabase.CreateAsset(mat, matPath);
                Debug.Log($"Created: {matName}\t{matPath}");
            }
			else
			{
                mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
			}
            return mat;
        }

        private static Shader GetPipelineShader()
        {
            RenderPipelineAsset pipelineAsset = GraphicsSettings.currentRenderPipeline;
            string pipelineAssetName = pipelineAsset.GetType().Name;
            if (pipelineAssetName.Contains("Universal"))
            {
                return Shader.Find("Universal Render Pipeline/Lit");
            }
            else if (pipelineAssetName.Contains("HDRP"))
            {
                return Shader.Find("HDRP/Lit");
            }

            return Shader.Find("Standard");
        }

    }
}