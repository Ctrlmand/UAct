using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
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
			if (!TryRead(sheetPath, out var worksheet, out var sharedStrings))
			{
				Debug.LogWarning("Not a good Excel file or file not found.");
				fieldData = null;
				return;
			}

			var rows = ParseWorksheet(worksheet, sharedStrings);
			if (rows.Count < 2)
			{
				fieldData = null;
				return;
			}

			var fieldNames = rows[0];
			var fieldTypes = rows[1];

			fieldData = new List<(object fieldType, object fieldName)>();
			for (int i = 0; i < fieldNames.Count; i++)
			{
				fieldData.Add((fieldTypes[i], fieldNames[i]));
			}
		}

		/// <summary>
		/// Out field's values.
		/// </summary>
		/// <param name="sheetPath"></param>
		/// <param name="FieldValues"></param>
		public static void ReadValues(string sheetPath, out object[,] FieldValues)
		{
			if (!TryRead(sheetPath, out var worksheet, out var sharedStrings))
			{
				Debug.LogWarning("Not a good Excel file or file not found.");
				FieldValues = null;
				return;
			}

			var rows = ParseWorksheet(worksheet, sharedStrings);
			if (rows.Count < 3)
			{
				FieldValues = null;
				return;
			}

			int rowCount = rows.Count - 2;
			int colCount = rows[0].Count;
			FieldValues = new object[rowCount, colCount];

			for (int i = 2; i < rows.Count; i++)
			{
				for (int j = 0; j < rows[i].Count; j++)
				{
					FieldValues[i - 2, j] = rows[i][j];
				}
			}
		}

		public static object[] ReadOneRow(string sheetPath, int rowIndex = 1, int startCol = 1)
		{
			if (!TryRead(sheetPath, out var worksheet, out var sharedStrings))
			{
				Debug.LogWarning("Not a good Excel file or file not found.");
				return null;
			}

			var rows = ParseWorksheet(worksheet, sharedStrings);
			if (rowIndex > rows.Count)
			{
				return new object[0];
			}

			var row = rows[rowIndex - 1];
			if (startCol > row.Count)
			{
				return new object[0];
			}

			int resultSize = row.Count - startCol + 1;
			object[] data = new object[resultSize];
			for (int i = startCol - 1; i < row.Count; i++)
			{
				data[i - startCol + 1] = row[i];
			}

			return data;
		}

		public static bool TryRead(string filePath, out XElement worksheet, out List<string> sharedStrings)
		{
			worksheet = null;
			sharedStrings = new List<string>();

			if (!File.Exists(filePath))
				return false;

			if (Path.GetExtension(filePath) != ".xlsx")
				return false;

			try
			{
				using (var archive = ZipFile.OpenRead(filePath))
				{
					var sheetEntry = archive.Entries.FirstOrDefault(e => e.FullName.EndsWith("sheet1.xml"));
					if (sheetEntry == null)
						return false;

					using (var stream = sheetEntry.Open())
					{
						worksheet = XElement.Load(stream);
					}

					var sharedStringsEntry = archive.Entries.FirstOrDefault(e => e.FullName.EndsWith("sharedStrings.xml"));
					if (sharedStringsEntry != null)
					{
						using (var stream = sharedStringsEntry.Open())
						{
							var sharedStringsDoc = XDocument.Load(stream);
							XNamespace ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
							sharedStrings = sharedStringsDoc.Descendants(ns + "t").Select(e => e.Value).ToList();
						}
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				Debug.LogError($"Failed to read Excel file: {ex.Message}");
				return false;
			}
		}

		public static object[,] ReadRow(string sheetPath, int startRow = 1, int startCol = 1)
		{
			if (!TryRead(sheetPath, out var worksheet, out var sharedStrings))
			{
				Debug.LogWarning("Not a good Excel file or file not found.");
				return null;
			}

			var rows = ParseWorksheet(worksheet, sharedStrings);
			if (rows.Count < startRow)
			{
				return new object[0, 0];
			}

			int rowCount = rows.Count - startRow + 1;
			int colCount = rows.Max(r => r.Count);
			object[,] data = new object[rowCount, colCount];

			for (int i = startRow - 1; i < rows.Count; i++)
			{
				for (int j = startCol - 1; j < rows[i].Count; j++)
				{
					data[i - startRow + 1, j] = rows[i][j];
				}
			}

			return data;
		}

		private static List<List<object>> ParseWorksheet(XElement worksheet, List<string> sharedStrings)
		{
			XNamespace ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
			var rows = new List<List<object>>();

			var rowElements = worksheet.Descendants(ns + "row");
			foreach (var rowElement in rowElements)
			{
				var cells = new List<object>();
				var cellElements = rowElement.Descendants(ns + "c");

				int lastColumn = 0;
				foreach (var cellElement in cellElements)
				{
					string cellReference = cellElement.Attribute("r")?.Value;
					if (cellReference != null)
					{
						int currentColumn = GetColumnIndex(cellReference);
						while (cells.Count < currentColumn - 1)
						{
							cells.Add("");
						}
					}

					string cellValue = GetCellValue(cellElement, ns, sharedStrings);
					cells.Add(cellValue);
				}

				if (cells.Count > 0)
					rows.Add(cells);
			}

			return rows;
		}

		private static string GetCellValue(XElement cellElement, XNamespace ns, List<string> sharedStrings)
		{
			var valueElement = cellElement.Element(ns + "v");
			if (valueElement == null)
				return "";

			string value = valueElement.Value;
			string type = cellElement.Attribute("t")?.Value;

			if (type == "s")
			{
				int index;
				if (int.TryParse(value, out index) && index >= 0 && index < sharedStrings.Count)
				{
					return sharedStrings[index];
				}
				return value;
			}
			else if (type == "b")
			{
				return value == "1" ? "true" : "false";
			}
			else if (type == "str")
			{
				return value;
			}

			return value;
		}

		private static int GetColumnIndex(string cellReference)
		{
			string columnLetters = new string(cellReference.Where(char.IsLetter).ToArray());
			int columnIndex = 0;
			for (int i = 0; i < columnLetters.Length; i++)
			{
				columnIndex = columnIndex * 26 + (columnLetters[i] - 'A' + 1);
			}
			return columnIndex;
		}
	}
}
