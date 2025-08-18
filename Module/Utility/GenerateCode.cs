using System;
using System.IO;
using System.Reflection;

namespace UAct.Util
{
	public static class CodeGenerate
	{
		public static string Namespace = "UAct.Generated";

		/// <summary>
		/// Return the name of the GeneratedClass
		/// </summary>
		/// <param name="sheetPath"></param>
		/// <returns></returns>
		public static string GetSOTypeName(string sheetPath)
		{
			string sheetName = Path.GetFileNameWithoutExtension(sheetPath);
			string className = $"{sheetName}SO";

			return className;
		}

		/// <summary>
		/// Return the name of the DataItem in GeneratedClass.
		/// </summary>
		/// <param name="className"></param>
		/// <returns></returns>
		public static string GetSODataItemName(string className)
		{
			string itemClassName = $"{className}Item";
			return itemClassName;
		}

		/// <summary>
		/// Try get type in Assembly-CSharp.
		/// If not exist, Log info and return null.
		/// </summary>
		/// <param name="className"></param>
		/// <param name="type">out type value</param>
		/// <returns></returns>
		public static Type TryGetTypeInAssembly(string className)
		{
			Type type;

			type = Type.GetType(className);
			if (type != null)
			{
				return type;
			}
			// Try to find in Assembly-CSharp assembly
			Assembly assembly = Assembly.Load("Assembly-CSharp");
			if (assembly != null)
			{
				type = assembly.GetType(className);
				if (type != null)
				{
					// Debug.Log($"Find it In <{assembly.FullName}>");
					return type;
				}
			}

			return type;
		}

	}
}