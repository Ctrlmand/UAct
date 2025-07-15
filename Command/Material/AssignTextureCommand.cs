using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Extension.Editor.Framwork;

namespace Extension.Editor.Tools.Batch
{
    public class AssignTextureCommand : ICommand
    {
        public void Execute()
        {
            // 获取选中的对象
            Object[] selectedObjects = Selection.objects;

            foreach (Object obj in selectedObjects)
            {
                if (obj is Material material)
                {
                    ProcessMaterial(material);
                }
                else
                {
                    Debug.LogWarning($"{obj.name} is not a Material.");
                }
            }

            Debug.Log("Texture assignment completed.");
        }
        //-------------------------------------------------------------------------------------------
        private void ProcessMaterial(Material material)
        {
            if (material == null)
            {
                Debug.LogWarning("Material is null.");
                return;
            }

            string materialName = material.name.ToString();
            // 定义贴图命名规则
            var textureProperties = new Dictionary<string, string>{
            { "DpR", "_MaskMap" },
            { "D", "_BaseColorMap" },
            { "N", "_NormalMap"},
        };

            // 查找资源文件夹中的所有纹理
            string[] allTextures = AssetDatabase.FindAssets("t:Texture");

            foreach (var property in textureProperties)
            {
                string textureType = property.Key;
                string propertyName = property.Value;
                // 检查材质是否具有该属性
                if (!material.HasProperty(propertyName))
                {
                    continue;
                }
                // 构造贴图名称匹配规则
                string materialPrefix = "MI_";
                materialName = UtilFunc.String.RemovePrefix(materialName, materialPrefix);
                string expectedTextureName = $"T_{materialName}_{textureType}";
                Debug.Log(expectedTextureName);
                // 查找符合条件的贴图
                Texture2D matchingTexture = FindTexture(expectedTextureName, allTextures);

                if (matchingTexture != null)
                {
                    material.SetTexture(propertyName, matchingTexture);
                }
                else
                {
                    Debug.LogWarning($"NotFound >>{textureType}<< {material.name}.");
                }
            }
        }

        private Texture2D FindTexture(string expectedTextureName, string[] allTextures)
        {
            foreach (string assetGUID in allTextures)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);

                if (texture != null && texture.name.ToLower().Contains(expectedTextureName.ToLower()))
                {
                    return texture;
                }
            }

            return null;
        }
    }
}