using UnityEditor;
using UnityEngine;

namespace UAct.Batch
{
    
    public class InvertGChannelCommand : ICommand
    {
        public void Execute(ICommandContext context)
        {
            // 获取选中的对象
            Object[] selectedObjects = Selection.objects;
            foreach (Object obj in selectedObjects)
            {
                if (obj is Texture2D texture)
                {
                    InvertGreenChannel(texture);
                }
                else
                {
                    Debug.LogWarning($"Selected object {obj.name} is not a Texture2D.");
                }
            }

            return;
        }
        private void InvertGreenChannel(Texture2D texture)
        {
            if (texture == null)
            {
                Debug.LogWarning("Texture is null.");
                return;
            }
            TextureImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture)) as TextureImporter;

            if (importer.swizzleG == TextureImporterSwizzle.OneMinusG)
            {
                importer.swizzleG = TextureImporterSwizzle.G;
            } else if (importer.swizzleG == TextureImporterSwizzle.G)
            {
                importer.swizzleG = TextureImporterSwizzle.OneMinusG;
                Debug.Log(importer.swizzleG);
            }
            importer.SaveAndReimport();

            // Debug.Log($"Inverted G channel: {texture.name} done!");
        }

    }
}