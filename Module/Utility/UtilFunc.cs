using UnityEngine;
using UnityEditor;
using System.IO;

namespace Extension.Editor
{
    public static class UtilFunc
    {
        public static class ModelImporter
        {
            public static void SearchAndRemapMaterials(Object obj)
            {
                UnityEditor.ModelImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(obj)) as UnityEditor.ModelImporter;
                importer.SearchAndRemapMaterials(ModelImporterMaterialName.BasedOnMaterialName, ModelImporterMaterialSearch.RecursiveUp);
                // importer.SaveAndReimport();

                // Debug.Log("SearchAndRemapMaterials");
            }
        }

        public static class String
        {
            public static string RemovePrefix(string name, string prefix)
            {
                if (name.StartsWith(prefix))
                {
                    return name.Substring(prefix.Length);
                }
                return name;
            }

        }

        public static class Assets
        {
            public static bool AssetsExitAtPath(string path)
            {

                string guid = AssetDatabase.AssetPathToGUID(path, AssetPathToGUIDOptions.OnlyExistingAssets);
                if (guid == string.Empty) return false;

                return true;
            }
            public static void GetSelectedAssetPath()
            {
                Object active = Selection.activeObject;
                Debug.Log(AssetDatabase.GetAssetPath(active));

            }

            public static void GetSelectAssetType()
            {
                Object active = Selection.activeObject;
                Debug.Log(active.GetType().Name);
            }
            // public static void RenameAsParent()
            // {
            //     Object active = Selection.activeObject;
            //     string activePath = AssetDatabase.GetAssetPath(active);
            //     string newName = Path.GetFileName(Path.GetDirectoryName(activePath));
            //     AssetDatabase.RenameAsset(activePath, newName);
            // }

            // public static void RemapMaterials()
            // {
            //     GameObject[] selected = Selection.gameObjects;
            //     foreach (Object obj in selected)
            //     {
            //         ModelImporter.SearchAndRemapMaterials(obj);
            //     }
            // }
        }






    }
}