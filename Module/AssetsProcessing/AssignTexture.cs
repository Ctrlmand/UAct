using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace UAct.AssetsProcessing
{
	using Util;
    
    public class AssignTexture : ICommand
    {

        private static string DEFAULT_MAP_PATH = $"{AssetMethod.GetPresetPath()}/DefaultMap.json";

        public void Execute(ICommandContext context)
        {
            // get context data
            bool isUseConfigFile = context.GetData<bool>();
            Object jsonObject = (context.GetData<Object>() != null) ? context.GetData<Object>() : LoadDefaultPreset();
            string jsonPath = AssetDatabase.GetAssetPath(jsonObject);
            string[] mapInfo_prefix = context.GetData<string[]>();

            if (mapInfo_prefix.Length != 3)
            {
                Debug.LogError("mapInfo_prefix must be 3 elements.");
                return;
            }

            string mapInfo = mapInfo_prefix[0];
            string matPrefix = mapInfo_prefix[1];
            string texPrefix = mapInfo_prefix[2];

            if (matPrefix.Length == 0) matPrefix = "/";
            if (texPrefix.Length == 0) texPrefix = "/";

            foreach (Object obj in Selection.objects)
            {
                if (obj is Material mat)
                {
                    // Get map
                    Dictionary<string, string> texPropMap;
                    texPropMap = isUseConfigFile ? SerializeData.ReadDictionaryJson(jsonPath) : ReadMapInfo(mapInfo);
                    if (texPropMap == null) return;
                    // Do it
                    ProcessMaterial(mat, texPropMap, matPrefix, texPrefix);
                }
                else
                {
                    Debug.LogWarning($"{obj.name} is not a Material.");
                    return;
                }
            }

            return;
        }
        //-------------------------------------------------------------------------------------------
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

        public static void GenerateMapConfigBySelected(ref string mapInfo)
        {
            Object active = Selection.activeObject;
            if (active.GetType() != typeof(Material))
            {
                Debug.LogError($"Selecte a {active.GetType()}, Please select a Material.");
                return;
            }
            Material mat = active as Material;
            string targetMap = "";
            Shader shader = mat.shader;

            int propertyCount = shader.GetPropertyCount();
            for (int i = 0; i < propertyCount; i++)
            {
                string propType = shader.GetPropertyType(i).ToString();
                if (propType != "Texture") continue;
                string propName = shader.GetPropertyName(i);
                if (propName.StartsWith("unity")) continue;
                targetMap += $"{propName.Replace("_", "")} => {propName}\n";
            }

            mapInfo = targetMap;
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
                string assetPath = DEFAULT_MAP_PATH.Replace(prefixDirString + "/", string.Empty);
                return AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            }

            // Default preset exists
            // Create default preset file
            Dictionary<string, string> dict;
            dict = new Dictionary<string, string>()
            {
                { "_AORM", "AORM" },
                { "_BaseColor", "BaseColor" },
                { "_Normal", "Normal" },
                { "_Emission", "Emission" },
            };
            string jsonString = SerializeData.DictionaryToJson(dict);

            File.WriteAllText(DEFAULT_MAP_PATH, jsonString);
            AssetDatabase.Refresh();

            return AssetDatabase.LoadAssetAtPath<Object>(DEFAULT_MAP_PATH);

        }

        private void ProcessMaterial(Material mat, Dictionary<string, string> texPropMap, string matPrefix = "", string texPrefix = "")
        {
            // Get Textures folder path
            string matPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(mat));
            string texDir = AssetMethod.RecursiveSearchDir(matPath, "Textures");
            // Not found Textures Directory
            if (texDir.Length == 0) return;

            // Traverse texture properties
            foreach (var property in texPropMap)
            {
                // Debug.Log($"propertyName: {property.Key}, textureType: {property.Value}");
                string propertyName = property.Key;
                string textureType = property.Value;
                
                // not has property
                if (!mat.HasProperty(propertyName))
                {
					Debug.LogWarning($"[{mat.name}] No Property: {propertyName}.");
                    continue;
				}
                

                // Construct expected texture name
                string expectedTextureName = $"{texPrefix}{mat.name.Replace(matPrefix, "")}_{textureType}".Replace("/", "");
                // Debug.Log($"expectedTexName: {expectedTextureName}");

                // Found texture
                Texture2D matchingTexture = FindTexture(expectedTextureName, texDir);
                if (matchingTexture != null)
                {
                    mat.SetTexture(propertyName, matchingTexture);
                    if (textureType.Equals("Normal")) TexUtil.CheckTextureType(matchingTexture, TextureImporterType.NormalMap);
                    // Debug.Log($"[{mat.name}]FoundTex: {textureType}");
                    continue;
                }

                // Not found
                Debug.LogWarning($"[{mat.name}] NotFound: {expectedTextureName}.");

            }

        }

        private static Dictionary<string, string> ReadMapInfo(string mapInfo)
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            if (mapInfo.EndsWith("\n"))
            {
                mapInfo = mapInfo.Substring(0, mapInfo.Length - 1);
            }
            string[] mapData = mapInfo.Split("\n");
            foreach (string rowData in mapData)
            {
                if (string.IsNullOrWhiteSpace(rowData)) continue;

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
        public AssignTextureContext(bool isUseConfigFile, Object jsonObject, string[] mapInfo_prefix)
        {
            SetData(isUseConfigFile);
            SetData(jsonObject);
            SetData(mapInfo_prefix);
        }
    }
}
