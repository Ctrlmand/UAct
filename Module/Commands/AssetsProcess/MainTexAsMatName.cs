using UnityEngine;
using UnityEditor;
using System.IO;

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
                    string firstTexName = GetFirstTexName(material);
                    if (string.IsNullOrEmpty(firstTexName)) continue;

                    string srcFilePath = AssetDatabase.GetAssetPath(material);

                    RecursiveRename(srcFilePath, firstTexName);
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

        private void RecursiveRename(string srcFilePath, string targetFileName, int suffix = 1)
		{
            string directoryName = Path.GetDirectoryName(srcFilePath);
            string fileExtension = Path.GetExtension(srcFilePath);
            string newAssetPath = Path.Combine(directoryName, targetFileName + fileExtension);
            if (!File.Exists(newAssetPath))
            {
                AssetDatabase.RenameAsset(srcFilePath, targetFileName);
            }
            else
            {
                newAssetPath = Path.Combine(directoryName, targetFileName + "_" + suffix + fileExtension);
                if (!File.Exists(newAssetPath))
                {
                    AssetDatabase.RenameAsset(srcFilePath, targetFileName + "_" + suffix);
                }
                else
                {
                    RecursiveRename(srcFilePath, targetFileName, suffix + 1);
                }
            }

		}
	}
}
