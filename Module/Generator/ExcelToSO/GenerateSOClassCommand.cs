using System.Text;
using UnityEngine;

namespace UAct.Generator
{
	using System.Collections.Generic;
	using System.IO;
	using UnityEditor;
	using Util;

	public class GenerateSOClassCommand : ICommand
	{
		public void Execute(ICommandContext context)
		{
			string filePath = context.GetData<string>();
			string className = CodeGenerate.GetSOTypeName(filePath);

			// Read field names and types
			ExcelRead.ReadField(filePath, out List<(object, object)> fieldInfo);
			if (fieldInfo == null || fieldInfo.Count == 0) return;

			StringBuilder sb = new StringBuilder();
			// Generate file header
			GeneratedFileHeader(sb);

			// Generate data item class
			GenerateItemClass(sb, className, fieldInfo);
			sb.AppendLine("}");

			// Make directory exist
			string savePath = $"{AssetMethod.GetGeneratFolder()}/ScriptableObjec/{className}.cs";
			string directory = Path.GetDirectoryName(savePath);
			if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
			
			// Write to file
			File.WriteAllText(savePath, sb.ToString());
			AssetDatabase.Refresh();

			// Log save path
			EditorApplication.delayCall += () =>
			{
				Debug.Log($"Generated instance At: {savePath}");
			};

			return;
		}

		private static void GeneratedFileHeader(StringBuilder sb)
		{
			// Head comment
			sb.AppendLine("// This Code is Auto Generated, Please Do Not Modify");
			sb.AppendLine($"// Generated at: {System.DateTime.Now}");
			sb.AppendLine();

			// Using package
			sb.AppendLine("using System;");
			sb.AppendLine("using System.Collections.Generic;");
			sb.AppendLine("using UnityEngine;");
			sb.AppendLine();

			// Namespace
			sb.AppendLine($"namespace {CodeGenerate.Namespace}");
			sb.AppendLine("{");
		}

		private static void GenerateItemClass(StringBuilder sb, string className, List<(object, object)> fieldData)
		{
			string itemClassName = CodeGenerate.GetSODataItemName(className);

			sb.AppendLine("\t[Serializable]");
			sb.AppendLine($"\tpublic class {itemClassName}");
			sb.AppendLine("\t{");

			foreach (var item in fieldData)
			{
				
				string fieldType = item.Item1.ToString() ?? "string";
				string fieldName = item.Item2.ToString() ?? $"field{fieldData}";

				// Make sure the field name is a valid identifier
				fieldName = MakeValidIdentifier(fieldName);

				sb.AppendLine($"\t\tpublic {fieldType} {fieldName};");

			}

			sb.AppendLine("\t}");
			sb.AppendLine();

			// Generate ScriptableObject class
			sb.AppendLine($"\tpublic class {className} : ScriptableObject");
			sb.AppendLine("\t{");

				// Generate List field
				sb.AppendLine($"\t\tpublic List<{itemClassName}> items = new List<{itemClassName}>();");

			sb.AppendLine("\t}");
			sb.AppendLine();
		}


		private static string MakeValidIdentifier(string identifier)
		{
			// Remove illegal characters
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < identifier.Length; i++)
			{
				char c = identifier[i];
				if (i == 0 && char.IsDigit(c))
				{
					sb.Append('_');
				}

				if (char.IsLetterOrDigit(c) || c == '_')
				{
					sb.Append(c);
				}
			}

			return sb.ToString();
		}
	}
	public class GenerateSOClassContext : BaseCommandContext
	{
		public GenerateSOClassContext(string sheetPath)
		{
			SetData(sheetPath);
		}
	}

}
