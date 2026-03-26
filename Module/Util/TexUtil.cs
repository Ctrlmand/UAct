using UnityEngine;
using UnityEditor;

namespace UAct.Util
{

	public static class TexUtil
	{
		public static void CheckTextureType(Texture2D tex, TextureImporterType type)
		{
            TextureImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(tex)) as TextureImporter;
            if (importer.textureType != type)
			{
				importer.textureType = type;
				importer.SaveAndReimport();
			}

		}
	}
}