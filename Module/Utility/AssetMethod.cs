using UnityEngine;
using UnityEditor;
using System.IO;
using System.Runtime.CompilerServices;

namespace UAct.Util
{
    public static class AssetMethod
    {

        public static string CreateMatFolderForObj(Object obj, string folderName)
        {
            string objPath = AssetDatabase.GetAssetPath(obj);
            string parentPath = Path.GetDirectoryName(objPath);
            string folderPath = $"{parentPath}/{folderName}";

            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(parentPath, folderName);
            }

            return folderPath;
        }

        public static string GetGeneratFolder()=> "Assets/Code/Generated";

        /// <summary>
        /// Return the path of the preset folder
        /// </summary>
        public static string GetPresetPath()=> Path.Combine(GetUActDir(), "Presets");
		
        public static string GetUActDir()
		{
			string dirPath = GetCallerPathInProject();
            dirPath = dirPath.Substring(0, dirPath.IndexOf("/UAct/") + 5);
			return dirPath;
		}
        public static string GetCallerPathInProject([CallerFilePath] string filePath = "")
        {
            return filePath.Replace(@"\", "/").Replace(Application.dataPath, "Assets");
        }
        /// <summary>
        /// Recursive search directory
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="dirName"></param>
        /// <returns></returns>
        public static string RecursiveSearchDir(string dirPath, string dirName)
        {
            if (!Directory.Exists(dirPath)) return null;

            string[] subDirs = Directory.GetDirectories(dirPath);
            foreach (string dir in subDirs)
            {
                Debug.Log(dir);
                if (dir.EndsWith(dirName))
                {
                    return dir;
                }
            }

            string foundDir = RecursiveSearchDir(Path.GetDirectoryName(dirPath), dirName);
            return foundDir;
        }


    }
}