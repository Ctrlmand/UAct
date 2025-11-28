using UnityEngine;
using UnityEditor;

namespace UAct.AssetsProcessing
{
	using Util;

	public class RenameAssetsCommand : ICommand
	{
		public void Execute(ICommandContext context)
		{
            // Get context
			string[] renameInfo = context.GetData<string[]>();
			string newName = renameInfo[0];
			string newNameSuffix = renameInfo[1];

            int count = 0;
            foreach (Object obj in Selection.objects)
			{
                string objPath = AssetDatabase.GetAssetPath(obj);

                // Construct the new name
                // If newName is '*', use the original name of the object
                string newObjName = newName;
                if (string.IsNullOrEmpty(newObjName)) newObjName = obj.name; // Use original name if '*' is specified
                else newObjName = $"{newObjName}_{count:D3}";

                if (newNameSuffix.Length > 0) newObjName += newNameSuffix;

                // Rename the asset
                AssetDatabase.RenameAsset(objPath, newObjName);
				count++;
			}
		}
	}
}