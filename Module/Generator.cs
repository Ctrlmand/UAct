using UnityEngine;
using UnityEditor;
using System.IO;

namespace UAct.Generator
{

	public class Generator : EditorWindowBase
	{
		// Uset Input
		private Object excelFile;
		private BaseCommandContext GetPathContext()
		{
			string dataPath = Path.GetDirectoryName(Application.dataPath);
			string filePath = AssetDatabase.GetAssetPath(excelFile);
			string fullPath = Path.Combine(dataPath, filePath).Replace('\\', '/');

			BaseCommandContext context = new BaseCommandContext();
			context.SetData(fullPath);
			return context;

		}

		// Menu
		[MenuItem("UAct/Generator", false, 11)]
		private static void ShowWindow()
		{
			EditorWindow window = GetWindow<Generator>();
			window.titleContent = new GUIContent("Generator");
			window.Show();
		}

		// Gui Function
		private void OnGUI()
		{
			GUILayout.Label("Generate SO From Excel", EditorStyles.boldLabel);
			excelFile = EditorGUILayout.ObjectField("Excel File", excelFile, typeof(Object), false);

			CommandButton<GenerateSOClassCommand>("Generate SO Class", GetPathContext());
			CommandButton<GenerateSOInstanceCommand>("Generate SO Instannce", GetPathContext());

		}

	}
}
