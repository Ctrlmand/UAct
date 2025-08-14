using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace UAct.Batch
{

	using Util;
    public class AssignTextureCommand : ICommand
    {
        private static string DEFAULT_MAP_PATH = $"{AssetMethod.GetPresetPath()}/DefaultMap.json";

        public void Execute(ICommandContext context)
        {
            // get context data
            bool isUseConfigFile = context.GetData<bool>();
            Object jsonObject = (context.GetData<Object>() != null) ? context.GetData<Object>() : LoadDefaultPreset();
            string jsonPath = AssetDatabase.GetAssetPath(jsonObject);
            string mapInfo = context.GetData<string>();


            foreach (Object obj in Selection.objects)
            {
                if (obj is Material mat)
                {
                    // Get map
                    Dictionary<string, string> texPropMap;
                    texPropMap = isUseConfigFile ? SerializeData.ReadDictionaryJson(jsonPath) : ReadMapInfo(mapInfo);
                    if (texPropMap == null) return;
                    // Do it
                    ProcessMaterial(mat, texPropMap);
                }
                else
                {
                    Debug.LogWarning($"{obj.name} is not a Material.");
                    return;
                }
            }

            return;
        }

        public static void StorePreset(string mapInfo)
        {
            string filePath = EditorUtility.SaveFilePanelInProject("Save Preset", "MapPreset", "json", "Save Preset");
            if (string.IsNullOrEmpty(filePath)) return;

            Dictionary<string, string> map = ReadMapInfo(mapInfo);
            if (map.Count == 0)
            {
                Debug.LogError("map is empty");
                return;
            }

            string jsonContent = SerializeData.DictionaryToJson(map);

            File.WriteAllText(filePath, jsonContent);
            AssetDatabase.Refresh();

            Debug.Log($"Done.");
        }
        //-------------------------------------------------------------------------------------------

        private Texture2D FindTexture(string expectedTextureName, string texDir)
        {
            string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { texDir });
            foreach (string assetGUID in textureGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);

                if (texture != null && texture.name.ToLower().EndsWith(expectedTextureName.ToLower()))
                {
                    return texture;
                }
            }

            return null;
        }
        private Object LoadDefaultPreset()
        {
            // Default preset exists
            // Load default preset file
            if (File.Exists(DEFAULT_MAP_PATH))
            {
                string prefixDirString = Path.GetDirectoryName(Application.dataPath).Replace(@"\", "/");
                string assetPath  = DEFAULT_MAP_PATH.Replace(prefixDirString+"/", string.Empty);
                return AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            }

            // Default preset exists
            // Create default preset file
            Dictionary<string, string> dict;
            dict = new Dictionary<string, string>()
            {
                { "_BaseMap", "BaseMap" },
                { "_MetallicGlossMap", "Metallic" },
                { "_BumpMap", "Normal" },
                { "_ParallaxMap", "Height" },
                { "_OcclusionMap", "AO" },
                { "_EmissionMap", "Emission" },
            };
            string jsonString = SerializeData.DictionaryToJson(dict);

            File.WriteAllText(DEFAULT_MAP_PATH, jsonString);
            AssetDatabase.Refresh();

            return AssetDatabase.LoadAssetAtPath<Object>(DEFAULT_MAP_PATH);

        }

        private void ProcessMaterial(Material mat, Dictionary<string, string> texPropMap)
        {
            // Get Textures folder path
            string matPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(mat));
            string texDir = AssetMethod.RecursiveSearchDir(matPath, "Textures");
            // Not found Textures Directory
            if (texDir.Length == 0) return;

            // Traverse texture properties
            foreach (var property in texPropMap)
            {
                string propertyName = property.Key;
                string textureType = property.Value;
                // not has property
                if (!mat.HasProperty(propertyName)) continue;

                // Construct expected texture name
                string expectedTextureName = $"{mat.name}_{textureType}";
                // Debug.Log($"expectedTexName: {expectedTextureName}");

                // Found texture
                Texture2D matchingTexture = FindTexture(expectedTextureName, texDir);
                if (matchingTexture != null)
                {
                    mat.SetTexture(propertyName, matchingTexture);
                    // Debug.Log($"[{mat.name}]FoundTex: {textureType}");
                    continue;
                }

                // Not found
                Debug.LogWarning($"[{mat.name}] NotFound: {textureType}.");

            }

        }

        private static Dictionary<string, string> ReadMapInfo(string mapInfo)
        {
            Dictionary<string, string> map = new();
            string[] mapData = mapInfo.Split("\n");
            foreach (string rowData in mapData)
            {
                string[] itemData = rowData.Split("=>");
                if (itemData.Length != 2)
                {
                    Debug.LogError("MapInfo not good!");
                    return null;
                }
                map.Add(itemData[1].Trim(), itemData[0].Trim());

            }
            return map;
        }
    }

    public class AssignTextureContext : BaseCommandContext
    {
        public AssignTextureContext(bool isUseConfigFile, Object jsonObject,string mapInfo)
        {
            SetData(isUseConfigFile);
            SetData(jsonObject);
            SetData(mapInfo);
        }
    }
}
