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
			string fullClassName = $"{CodeGenerate.Namespace}.{className}";
			Debug.Log(fullClassName);
			Type soType = CodeGenerate.TryGetTypeInAssembly(fullClassName);
			if (soType == null)
			{
				Debug.LogWarning($"{className} not found, Try generate code or wate for compile.");
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
			for (int i = 0; i < values.GetLength(0); i++)
			{
				var item = Activator.CreateInstance(itemType);

				for (int j = 0; j < itemFields.Length; j++)
				{
					// Get item's fields and populate them
					FieldInfo field = itemFields[j];
					object value = values[i, j];

					// Not good value.
					if (value == null)
					{
						Debug.LogWarning($"Value for field {field.Name} is null.");
					}

					// If field type is not same as value type
					// Convert value to field type
					if (field.FieldType != value.GetType())
					{
						switch (field.FieldType.Name)
						{
							case "Int32":
								field.SetValue(item, Convert.ToInt32(value));
								break;
							case "Single":
								field.SetValue(item, Convert.ToSingle(value));
								break;
							case "String":
								field.SetValue(item, Convert.ToString(value));
								break;
							case "Boolean":
								field.SetValue(item, Convert.ToBoolean(value));
								break;
							case "Vector3":
								string[] strValue;
								strValue = value.ToString().Replace("(", "").Replace(")", "").Split(',');
								// foreach (string str in strValue)
								// {
								// 	Debug.Log(str);
								// }
								field.SetValue(item, new Vector3(float.Parse(strValue[0]), float.Parse(strValue[1]), float.Parse(strValue[2])));
								break;
							default:
								Debug.LogWarning($"Unsupported field type: {field.FieldType.Name}");
								break;
						}
					}
					else
					{
						field.SetValue(item, value);
					}

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