using System.Collections.Generic;
using System.IO;

namespace UAct.Util
{
	public static class SerializeData
	{
		public static string DictionaryToJson(Dictionary<string, string> dict)
		{
			if (dict == null || dict.Count == 0)
				return "{}";

			// Build JSON info 
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.AppendLine("{\n\t\"" + "Dictionary<string, string>" + "\":\n\t{");

			int count = 0;
			foreach (var item in dict)
			{
				count++;

				// Write key and value
				string key = item.Key;
				string value = item.Value;
				sb.Append($"\t\t\"{key}\": \"{value}\"");

				// if not last
				if (count < dict.Count)
				{
					sb.AppendLine(",");
				}
				else
				{
					sb.AppendLine();
				}
			}

			sb.AppendLine("\t}\n}");
			return sb.ToString();
		}
		public static Dictionary<string, string> ReadDictionaryJson(string filePath)
		{
			// Debug.Log("Converting Json to Dictionary...");
			if (!filePath.EndsWith(".json") || !File.Exists(filePath)) return null;
			string json = File.ReadAllText(filePath);

			Dictionary<string, string> dict = new();
			string cleanJson = json.Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty);
			if (!cleanJson.StartsWith("{\"Dictionary<string,string>\"") || !cleanJson.EndsWith("}}"))
			{
				return null;
			}

			// string innerContent = cleanJson.Substring(cleanJson.IndexOf(":{") + 1, cleanJson.LastIndexOf("}}") - 1);
			string innerJson = cleanJson.Replace("\"Dictionary<string,string>\":", string.Empty).Replace("}", string.Empty).Replace("{", string.Empty);
			string[] dataJson = innerJson.Split(',');
			foreach (var item in dataJson)
			{
				string[] keyValue = item.Replace("\"", string.Empty).Split(':');
				dict.Add(keyValue[0], keyValue[1]);
			}

			return dict;
		}
	}
}