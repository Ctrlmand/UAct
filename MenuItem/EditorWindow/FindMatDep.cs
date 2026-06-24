
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace UAct.MenuWindow
{
	public class FindMatDeps : EditorWindowBase<FindMatDeps>
	{
		private GameObject targetObject;
		private DefaultAsset excludeDirectory;
		private List<Material> foundMat = new List<Material>();
		private string excludeDirectoryPath;

		const int menuItemIdCounter = 50;

		[MenuItem("Assets/Find Dependencies", false, menuItemIdCounter)]
		public static void FindMatDependencies()
		{
			GameObject go = Selection.activeGameObject;

			var window = GetWindow<FindMatDeps>();
			window.titleContent = new GUIContent("Find Material Dependencies");
			window.targetObject = go;
			window.Show();
		
		}

		void OnGUI()
		{
			GUILayout.Label("target Object");
			targetObject = EditorGUILayout.ObjectField(targetObject, typeof(GameObject), false) as GameObject;
			excludeDirectory = EditorGUILayout.ObjectField(excludeDirectory, typeof(DefaultAsset), false) as DefaultAsset;
			excludeDirectoryPath = excludeDirectory!=null ? AssetDatabase.GetAssetPath(excludeDirectory) : null;

			if (GUILayout.Button("Find Material Dependencies"))
			{
				foundMat.Clear();
				GetMatDeps(targetObject, excludeDirectoryPath);
			}

			if (foundMat.Count > 0)
			{
				scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

				GUILayout.Label("Found Materials:");
				foreach (var mat in foundMat)
				{
					EditorGUILayout.ObjectField(mat, typeof(Material), false);
				}

				EditorGUILayout.EndScrollView();
			}

		}

		private void GetMatDeps(GameObject go, string excludeDirectoryPath)
		{
			string activePath = AssetDatabase.GetAssetPath(go);
			if (string.IsNullOrEmpty(activePath)) return;

			string[] dependencies = AssetDatabase.GetDependencies(activePath, true);

			foreach (string dep in dependencies)
			{
				if (!dep.StartsWith("Assets/")) continue;
				if (!dep.EndsWith(".mat")) continue;
				if (!string.IsNullOrEmpty(excludeDirectoryPath) && dep.StartsWith(excludeDirectoryPath)) continue;
				foundMat.Add(AssetDatabase.LoadAssetAtPath<Material>(dep));
			}


		}	


	}
}