using Extension.Editor.Framwork;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Extension.Editor.Tools.Batch
{
    public class ExtractMaterialCommand : ICommand
    {
        public void Execute()
        {
            GameObject[] selected = Selection.gameObjects;

            foreach (GameObject obj in selected)
            {
                string targetFolder = CreateTargetFolder(obj);
                List<Material> matList = GetEmbeddedMaterials(obj);

                foreach (Material mat in matList)
                {
                    // Debug.Log(mat.name);
                    TryCreateMaterial(targetFolder, mat.name);
                }
                UtilFunc.ModelImporter.SearchAndRemapMaterials(obj);
                

            }
            AssetDatabase.SaveAssets();

        }
        
        private List<Material> GetEmbeddedMaterials(Object obj)
        {
            string objPath = AssetDatabase.GetAssetPath(obj);
            Object[] dependencies = AssetDatabase.LoadAllAssetsAtPath(objPath);
            List<Material> materials = new List<Material>();

            foreach (Object item in dependencies)
            {
                if (item is Material)
                {
                    materials.Add(item as Material);
                }
            }
            return materials;
        }

        private void TryCreateMaterial(string folderPath, string matName)
        {
            folderPath = $"{folderPath}";
            Shader shader = Shader.Find("HDRP/Lit");

            if (shader == null)
            {
                Debug.LogError("Shader not found: HDRP/Lit");
                return;
            }

            Material mat = new Material(shader);
            string matPath = $"{folderPath}/{matName}.mat";

            // Debug.Log($"{matName}\t{matPath}");
            // Mat is Exit
            bool canCreate = !UtilFunc.Assets.AssetsExitAtPath(matPath);
            // Debug.Log($"cancreate:\t{canCreate}");
            if (canCreate)
            {
                AssetDatabase.CreateAsset(mat, matPath);
            }
            // else{
            //     Debug.Log(materialName);
            // }

        }
        private static string CreateTargetFolder(Object obj)
        {
            string objPath = AssetDatabase.GetAssetPath(obj);
            string parentPath = Path.GetDirectoryName(objPath);
            string folderName = "Materials";
            string folderPath = $"{parentPath}/{folderName}";

            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(parentPath, folderName);
            }

            return folderPath;
        }


    }
}