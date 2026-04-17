using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace UAct.Generator
{
	using System.IO;
	using UnityEditor;
	using Util;

	public class GenerateSOInstanceCommand : ICommand
	{
		public void Execute(ICommandContext context)
		{
			string excelPath = context.GetData<string>();
			string soTypeName = CodeGenerate.GetSOTypeName(excelPath);
			ScriptableObject so = CreateSO(soTypeName);
			if (so == null)
			{
				Debug.LogWarning("Failed to create SO");
				return;
			}

			ExcelRead.ReadValues(excelPath, out object[,] values);
			Populate(so, values);

			return;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="className"></param>
		/// <returns></returns>
		private static ScriptableObject CreateSO(string className)
		{
			// Get SO Type
			string fullClassName = $"{CodeGenerate.Namespace}.{className}";
			Type soType = CodeGenerate.TryGetTypeInAssembly(fullClassName);
			if (soType == null)
			{
				Debug.LogError($"{className} not found, Try generate code or wate for compile.");
				return null;
			}

			// Create ScriptableObject Asset
			string assetDir = $"{AssetMethod.GetGeneratFolder()}/ScriptableObjec";
			string assetPath = $"{AssetMethod.GetGeneratFolder()}/ScriptableObjec/{className}.asset";
			if (!Directory.Exists(assetDir)) Directory.CreateDirectory(assetDir);

			ScriptableObject so = ScriptableObject.CreateInstance(soType);
			

			AssetDatabase.CreateAsset(so, assetPath);
			AssetDatabase.Refresh();
			return so;
		}

		/// <summary>
		/// Populate SO with values
		/// </summary>
		/// <param name="so"></param>
		/// <param name="values"></param>
		public static void Populate(ScriptableObject so, object[,] values)
		{
			Type soType = so.GetType();
			string fullClassName = $"{CodeGenerate.Namespace}.{CodeGenerate.GetSODataItemName(soType.Name)}";
			Type itemType = CodeGenerate.TryGetTypeInAssembly(fullClassName);
			if (itemType == null)
			{
				Debug.LogWarning($"Type not found: {fullClassName}");
				return;
			}

			// Create a list to hold the items
			Type listType = typeof(List<>).MakeGenericType(itemType);
			var list = Activator.CreateInstance(listType);
			var addmethod = list.GetType().GetMethod("Add");

			// Get the fields of the ItemType
			FieldInfo[] itemFields = itemType.GetFields();
			if (itemFields == null)
			{
				Debug.LogError("ItemType has no fields.");
			}
			if (values.GetLength(1) != itemFields.Length)
			{
				Debug.LogError("sheet data not match.");
				return;
			}

			// Populate the ScriptableObject with data from the sheet
			for (int row = 0; row < values.GetLength(0); row++)
			{
				// Foreach row, match a item 
				var item = Activator.CreateInstance(itemType);

				for (int col = 0; col < itemFields.Length; col++)
				{
					// Get item's fields and populate them
					FieldInfo field = itemFields[col];
					object value = values[row, col];

					// Not good value.
					if (value == null)
					{
						Debug.LogWarning($"Value for field {field.Name} is null.");
					}

					// If field type is not same as value type
					// Convert value to field type
					try
					{
						value = value.GetType() == field.FieldType ? value : Convert.ChangeType(value, field.FieldType);
					}
					catch (Exception e)
					{
						if (field.FieldType == typeof(Vector3))
						{
							string strValue = value.ToString();
							string[] numValue = strValue.Replace("(", "").Replace(")", "").Split(',');
							if (numValue.Length != 3) continue;
							float x = float.Parse(numValue[0]);
							float y = float.Parse(numValue[1]);
							float z = float.Parse(numValue[2]);
							value = new Vector3(x, y, z);
						}
						else
						{
							Debug.LogError("Failed to convert value to field type.\n" + e);
							return;
						}
							
						}
					field.SetValue(item, value);


				}

				addmethod.Invoke(list, new object[] { item });
			}

			FieldInfo soField = soType.GetFields()[0];
			if (soField == null)
			{
				Debug.LogWarning("SO field not found.");
				return;
			}


			soField.SetValue(so, list);


		}

	}
}