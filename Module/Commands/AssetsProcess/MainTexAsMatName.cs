using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.VersionControl;

namespace UAct.Command.AssetsProcess
{

	public class MainTexAsMatName : ICommand
	{
		public void Execute(ICommandContext context)
		{
            Object[] objects = context.GetData<Object[]>();
            if (objects == null)
            {
                objects = new Object[]{context.GetData<Object>()};
            }

			foreach(Object item in objects)
            {
                if (item is Material material)
                {
                    string assetPath = AssetDatabase.GetAssetPath(material);
                    string firstTexName = GetFirstTexName(material);

                    WarningIfFileExistsInDirectory(assetPath, firstTexName);
                    if (!string.IsNullOrEmpty(firstTexName)) AssetDatabase.RenameAsset(assetPath, firstTexName);
                    EditorGUIUtility.PingObject(material);
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

        private void WarningIfFileExistsInDirectory(string srcFilePath, string targetFileName)
		{
            string directoryName = Path.GetDirectoryName(srcFilePath);
            string fileExtension = Path.GetExtension(srcFilePath);
            string newAssetPath = Path.Combine(directoryName, targetFileName + fileExtension);
            if (File.Exists(newAssetPath))
            {
                Debug.LogWarning($"File already exists: {newAssetPath}.");
            }
            return;
		}
	}
}
