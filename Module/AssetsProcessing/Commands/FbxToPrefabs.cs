using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace UAct.AssetsProcessing
{

	public class FbxToPrefabs : ICommand
	{
		public void Execute(ICommandContext context)
		{
			if (!Directory.Exists("Assets/Prefabs")) Directory.CreateDirectory("Assets/Prefabs");
			string savePath = EditorUtility.OpenFolderPanel("Save Prefabs", "Assets/Prefabs", "");
			if (string.IsNullOrEmpty(savePath)) return;

			foreach (Object obj in Selection.objects)
			{
				if (!Directory.Exists($"{savePath}/{obj.name}")) Directory.CreateDirectory($"{savePath}/{obj.name}");

				string objPath = AssetDatabase.GetAssetPath(obj);
				ModelImporter importer = AssetImporter.GetAtPath(objPath) as ModelImporter;

				if (importer == null)
				{
					Debug.LogWarning($"{obj.name} has not ModelImporter.");
					continue;
				}

				GameObject fbxRoot = Object.Instantiate(obj) as GameObject;

				Transform rootTransform = new GameObject("ROOT").transform;
				Dictionary<string, Transform> existParents = new Dictionary<string, Transform>();

				int count = 0;
				while (count < fbxRoot.transform.childCount)
				{
					// Get child info
					Transform child = fbxRoot.transform.GetChild(count);
					string currentParentName = child.name.Split(".")[0];

					// Create new parent if not exist
					if (!existParents.ContainsKey(currentParentName))
					{
						Transform newParent = new GameObject(currentParentName).transform;
						newParent.SetParent(rootTransform);
						existParents.Add(newParent.name, newParent);
					}

					// Set child parent
					child.SetParent(existParents[currentParentName]);
					child.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
					child.transform.localScale = Vector3.one;

				}
				foreach (Transform item in existParents.Values)
				{
					PrefabUtility.SaveAsPrefabAsset(item.gameObject, $"{savePath}/{obj.name}/{item.name}.prefab");
				}

				// Clean up
				Object.DestroyImmediate(fbxRoot);
				Object.DestroyImmediate(rootTransform.gameObject);
			}

            return;
        }

		
	}
}