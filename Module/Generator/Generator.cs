using UnityEngine;
using UnityEditor;
using System.IO;

namespace UAct.Generator
{

	public class Generator : EditorWindowBase
	{
		// User Input
		private Object excelFile;
		private Object mdFile;
		private BaseCommandContext GetPathContext(Object file)
		{
			string dataPath = Path.GetDirectoryName(Application.dataPath);
			string filePath = AssetDatabase.GetAssetPath(file);
			string fullPath = $"{dataPath}/{filePath}";

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

			CommandButton<GenerateSOClassCommand>("Generate SO Class", GetPathContext(excelFile));
			CommandButton<GenerateSOInstanceCommand>("Generate SO Instannce", GetPathContext(excelFile));

			GUILayout.Space(10);
			GUILayout.Label("Convert Mermaid Class Diagram to C#", EditorStyles.boldLabel);
			mdFile = EditorGUILayout.ObjectField("Markdown File", mdFile, typeof(Object), false);

			CommandButton<MermaidToCSharpCommand>("Convert to C# Classes", GetPathContext(mdFile));

		}


	}
}
