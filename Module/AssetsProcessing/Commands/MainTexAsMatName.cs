using UnityEngine;
using UnityEditor;

namespace UAct.AssetsProcessing
{

	public class MainTexAsMatName : ICommand
	{
		public void Execute(ICommandContext context)
		{
			foreach(Object item in Selection.objects)
            {
                if (item is Material material)
                {
                    string assetPath = AssetDatabase.GetAssetPath(material);
                    string firstTexName = GetFirstTexName(material);
                    
                    if (!string.IsNullOrEmpty(firstTexName)) AssetDatabase.RenameAsset(assetPath, firstTexName);

                }
            }
		}

        private string GetFirstTexName(Material mat)
        {
            string[] texPropName = mat.GetPropertyNames(MaterialPropertyType.Texture);
            if (mat.GetTexture(texPropName[0]) != null)
            {
                return mat.GetTexture(texPropName[0]).name;
            }
            return null;
        }
	}
}
