using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UAct.Util
{
	public static class ExcelRead
	{
		
		/// <summary>
		/// Out Item's field; Item1 is fieldType, Item2 is fieldName
		/// </summary>
		/// <param name="sheetPath"></param>
		/// <param name="FieldValues"></param>
		/// <param name="fieldNames"></param>
		/// <param name="fieldTypes"></param>
		public static void ReadField(string sheetPath, out List<(object fieldType, object fieldName)> fieldData)
		{

			// Try to read the Excel file
			FileInfo fileInfo = TryRead(sheetPath);

			// Not A good file
			if (fileInfo == null)
			{
				Debug.LogWarning("Not a good Excel file or file not found.");
				fieldData = null;
				return;
			}

			using (ExcelPackage package = new ExcelPackage(fileInfo))
			{
				ExcelWorksheet sheet = package.Workbook.Worksheets[1];

				int rowCount = sheet.Dimension.Rows;
				int colCount = sheet.Dimension.Columns;

				object[] fieldTypes = ReadOneRow(sheet, 2);
				object[] fieldNames = ReadOneRow(sheet, 1);

				fieldData = new();
				for (int i = 0; i < fieldNames.Length; i++)
				{
					fieldData.Add((fieldTypes[i], fieldNames[i]));
				}
			}

		}

		/// <summary>
		/// Out field's values.
		/// </summary>
		/// <param name="sheetPath"></param>
		/// <param name="FieldValues"></param>
		public static void ReadValues(string sheetPath, out object[,] FieldValues)
		{

			// Try to read the Excel file
			FileInfo fileInfo = TryRead(sheetPath);

			// Not A good file
			if (fileInfo == null)
			{
				Debug.LogWarning("Not a good Excel file or file not found.");
				FieldValues = null;
				return;
			}

			using (ExcelPackage package = new ExcelPackage(fileInfo))
			{
				ExcelWorksheet sheet = package.Workbook.Worksheets[1];

				int rowCount = sheet.Dimension.Rows;
				int colCount = sheet.Dimension.Columns;

				FieldValues = ReadRow(sheet, 3);
			}

		}
		public static object[] ReadOneRow(ExcelWorksheet sheet, int rowIndex = 1, int startCol = 1)
		{
			int colCount = sheet.Dimension.Columns;
			object[] data = new object[colCount];
			for (int i = startCol; i <= colCount; i++)
			{
				data[i - 1] = sheet.GetValue(rowIndex, i);
				// Debug.Log(sheet.GetValue(rowIndex, i + 1));
			}
			return data;

		}

		public static FileInfo TryRead(string filePath)
		{
			// is File Exists
			if (!File.Exists(filePath))return null;

			// is target type
			if (Path.GetExtension(filePath) != ".xlsx") return null;
			

			return new FileInfo(filePath);
		}

		public static object[,] ReadRow(ExcelWorksheet sheet, int startRow = 1, int startCol = 1)
		{
			int colCount = sheet.Dimension.Columns;
			int rowCount = sheet.Dimension.Rows;

			object[,] data = new object[rowCount - startRow + 1, colCount];
			// Debug.Log($"startColumn: {startRow}\narrayRowCount: {data.GetLength(0)}\narrayColCount: {data.GetLength(1)}");

			for (int i = startRow; i <= rowCount; i++)
			{
				for (int j = startCol; j <= colCount; j++)
				{
					// Debug.Log($"i: {i-startRow}, j: {j}");
					data[i - startRow, j - startCol] = sheet.GetValue(i, j);
				}

			}

			return data;
		}

	}
}
