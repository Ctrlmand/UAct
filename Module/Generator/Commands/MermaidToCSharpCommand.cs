using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEditor;

namespace UAct.Generator
{
    /// <summary>
    /// Command to convert Mermaid class diagrams from markdown files to C# code
    /// </summary>
    public class MermaidToCSharpCommand : ICommand
    {
        // Precompiled regular expressions for better performance
        private static readonly Regex MermaidRegex = new Regex(@"```mermaid\s*classDiagram\s*([\s\S]*?)\s*```");
        private static readonly Regex MultiplicityRegex = new Regex(@"\s*[0-9]+\s*");
        private static readonly Regex RelationshipTypeRegex = new Regex(@"\s*o|\s*<|\s*>");
        
        /// <summary>
        /// Executes the command to convert Mermaid diagram to C# files
        /// </summary>
        /// <param name="context">Command context providing markdown file path</param>
        public void Execute(ICommandContext context)
        {
            try
            {
                // Get markdown file path
                string mdFilePath = context.GetData<string>();
                if (string.IsNullOrEmpty(mdFilePath) || !File.Exists(mdFilePath))
                {
                    Debug.LogWarning("Invalid markdown file path.");
                    return;
                }

                // Get save path
                string generatedFilePath = EditorUtility.SaveFolderPanel("Save C# Files", Application.dataPath + "/Code/Generated", "");
                if (string.IsNullOrEmpty(generatedFilePath)) return;
                generatedFilePath = $"{generatedFilePath}/{Path.GetFileNameWithoutExtension(mdFilePath)}";

                // Read markdown file content
                string mdContent = File.ReadAllText(mdFilePath);

                // Extract mermaid class diagram
                string mermaidContent = ExtractMermaidClassDiagram(mdContent);
                if (string.IsNullOrEmpty(mermaidContent))
                {
                    Debug.LogWarning("No mermaid class diagram found in the markdown file.");
                    return;
                }

                // Parse mermaid class diagram
                List<ClassInfo> classes = ParseMermaidClassDiagram(mermaidContent);
                if (classes == null || classes.Count == 0)
                {
                    Debug.LogWarning("Failed to parse mermaid class diagram.");
                    return;
                }

                // Generate C# files
                GenerateCSharpFiles(classes, generatedFilePath);

                AssetDatabase.Refresh();
                Debug.Log("Successfully converted mermaid class diagram to C# files.");
            }
            catch (Exception ex)
            {
                Debug.LogError("Error converting mermaid class diagram to C# files: " + ex.Message);
                Debug.LogError(ex.StackTrace);
            }
        }

        /// <summary>
        /// Extracts mermaid class diagram content from markdown
        /// </summary>
        /// <param name="mdContent">Markdown content</param>
        /// <returns>Extracted mermaid class diagram content</returns>
        private string ExtractMermaidClassDiagram(string mdContent)
        {
            Match match = MermaidRegex.Match(mdContent);
            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }
            return null;
        }

        /// <summary>
        /// Parses mermaid class diagram content into class information objects
        /// </summary>
        /// <param name="mermaidContent">Mermaid class diagram content</param>
        /// <returns>List of parsed class information objects</returns>
        private List<ClassInfo> ParseMermaidClassDiagram(string mermaidContent)
        {
            List<ClassInfo> classes = new List<ClassInfo>();
            Dictionary<string, ClassInfo> classMap = new Dictionary<string, ClassInfo>();
            ClassInfo currentClass = null;

            // Split content into lines
            string[] lines = mermaidContent.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            // First pass: create all class definitions
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();

                // Check for class definition
                if (trimmedLine.StartsWith("class ") && trimmedLine.Contains("{") && !trimmedLine.Contains("--"))
                {
                    // Extract class name
                    int classNameStart = "class ".Length;
                    int classNameEnd = trimmedLine.IndexOf('{');
                    if (classNameEnd > classNameStart)
                    {
                        string className = trimmedLine.Substring(classNameStart, classNameEnd - classNameStart).Trim();
                        currentClass = new ClassInfo(className);
                        classes.Add(currentClass);
                        classMap[className] = currentClass;
                    }
                }
                // End of class definition
                else if (trimmedLine == "}")
                {
                    currentClass = null;
                }
            }

            // Second pass: parse class members and relationships
            currentClass = null;
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();

                // Check for class members (attributes and methods)
                if (trimmedLine.StartsWith("class ") && trimmedLine.Contains("{") && !trimmedLine.Contains("--"))
                {
                    int classNameStart = "class ".Length;
                    int classNameEnd = trimmedLine.IndexOf('{');
                    string className = trimmedLine.Substring(classNameStart, classNameEnd - classNameStart).Trim();
                    currentClass = classMap[className];
                }
                else if (currentClass != null && !string.IsNullOrEmpty(trimmedLine) && trimmedLine != "}")
                {
                    // Check if it's a method (contains '()')
                    if (trimmedLine.Contains("()"))
                    {
                        // Parse method with possible return type and parameters
                        ParseMethod(currentClass, trimmedLine);
                    }
                    else if (!trimmedLine.Contains("--")) // Skip relationship lines
                    {
                        // Assume it's an attribute
                        ParseAttribute(currentClass, trimmedLine);
                    }
                }
                // Check for relationships
                else if (trimmedLine.Contains("--|>"))
                {
                    // Inheritance relationship
                    ParseInheritanceRelationship(classMap, trimmedLine);
                }
                else if (trimmedLine.Contains("--"))
                {
                    // Association relationship
                    ParseAssociationRelationship(classMap, trimmedLine);
                }
                // End of class definition
                else if (trimmedLine == "}")
                {
                    currentClass = null;
                }
            }

            return classes;
        }

        /// <summary>
        /// Parses class attributes from mermaid syntax
        /// </summary>
        /// <param name="classInfo">Class to add attribute to</param>
        /// <param name="attributeLine">Attribute line from mermaid diagram</param>
        private void ParseAttribute(ClassInfo classInfo, string attributeLine)
        {
            // Remove access modifiers
            string cleanLine = attributeLine.Trim('+', '-', '#', '~').Trim();
            
            // Handle possible initialization values (e.g., "int age = 30")
            string initialValue = null;
            if (cleanLine.Contains('='))
            {
                int equalsIndex = cleanLine.IndexOf('=');
                string valuePart = cleanLine.Substring(equalsIndex + 1).Trim(';', ' ', '"', '\'');
                initialValue = valuePart;
                cleanLine = cleanLine.Substring(0, equalsIndex).Trim();
            }
            
            // Parse type and name
            string[] parts = cleanLine.Split(new[] { ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
            {
                string type = GetCSharpType(parts[0]);
                string name = parts[1];
                classInfo.Attributes.Add(new AttributeInfo { Type = type, Name = name, InitialValue = initialValue });
            }
            else if (parts.Length == 1)
            {
                // If only name is provided, default to string type
                classInfo.Attributes.Add(new AttributeInfo { Type = "string", Name = parts[0], InitialValue = initialValue });
            }
        }

        private void ParseMethod(ClassInfo classInfo, string methodLine)
        {
            // Remove access modifiers
            string cleanLine = methodLine.Trim('+', '-', '#', '~').Trim();
            
            // Parse return type, method name and parameters
            string returnType = "void";
            string methodName = cleanLine;
            List<string> parameters = new List<string>();
            
            int parenStart = cleanLine.IndexOf('(');
            int parenEnd = cleanLine.LastIndexOf(')');
            
            if (parenStart > 0 && parenEnd > parenStart)
            {
                // Extract method name and possible return type
                string methodPart = cleanLine.Substring(0, parenStart).Trim();
                string[] methodParts = methodPart.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                
                if (methodParts.Length > 1)
                {
                    returnType = GetCSharpType(methodParts[0]);
                    methodName = methodParts[1];
                }
                else if (methodParts.Length == 1)
                {
                    methodName = methodParts[0];
                }
                
                // Extract parameters
                string paramsPart = cleanLine.Substring(parenStart + 1, parenEnd - parenStart - 1).Trim();
                if (!string.IsNullOrEmpty(paramsPart))
                {
                    string[] paramParts = paramsPart.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string param in paramParts)
                    {
                        string[] paramDetails = param.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (paramDetails.Length >= 2)
                        {
                            string paramType = GetCSharpType(paramDetails[0]);
                            string paramName = paramDetails[1];
                            parameters.Add($"{paramType} {paramName}");
                        }
                    }
                }
            }
            
            classInfo.Methods.Add(new MethodInfo { ReturnType = returnType, Name = methodName, Parameters = parameters });
        }

        private void ParseInheritanceRelationship(Dictionary<string, ClassInfo> classMap, string relationshipLine)
        {
            string[] parts = relationshipLine.Split(new[] { "--|>" }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                string derivedClassName = parts[0].Trim();
                string baseClassName = parts[1].Trim();
                
                // Remove possible multiplicity indicators
                derivedClassName = Regex.Replace(derivedClassName, @"\s*[0-9]+\s*", "").Trim();
                baseClassName = Regex.Replace(baseClassName, @"\s*[0-9]+\s*", "").Trim();
                
                if (classMap.ContainsKey(derivedClassName) && classMap.ContainsKey(baseClassName))
                {
                    classMap[derivedClassName].BaseClassName = baseClassName;
                }
            }
        }

        /// <summary>
        /// Parses association relationships between classes
        /// </summary>
        /// <param name="classMap">Map of class names to class objects</param>
        /// <param name="relationshipLine">Relationship line from mermaid diagram</param>
        private void ParseAssociationRelationship(Dictionary<string, ClassInfo> classMap, string relationshipLine)
        {
            string[] parts = relationshipLine.Split(new[] { "--" }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                string class1Name = parts[0].Trim();
                string class2Name = parts[1].Trim();
                
                // Extract relationship multiplicity and type
                string class1Multiplicity = ExtractMultiplicity(class1Name);
                string class2Multiplicity = ExtractMultiplicity(class2Name);
                
                // Remove possible multiplicity indicators and relationship types
                class1Name = MultiplicityRegex.Replace(class1Name, "").Trim();
                class2Name = MultiplicityRegex.Replace(class2Name, "").Trim();
                class1Name = RelationshipTypeRegex.Replace(class1Name, "").Trim();
                class2Name = RelationshipTypeRegex.Replace(class2Name, "").Trim();
                
                // Add appropriate references based on multiplicity
                if (classMap.ContainsKey(class1Name) && classMap.ContainsKey(class2Name))
                {
                    // Check if it's a one-to-many or many-to-many relationship
                    bool isClass1Collection = IsCollectionMultiplicity(class1Multiplicity);
                    bool isClass2Collection = IsCollectionMultiplicity(class2Multiplicity);
                    
                    // Add list property for many-side
                    if (isClass2Collection)
                    {
                        classMap[class1Name].Attributes.Add(new AttributeInfo { Type = $"List<{class2Name}>" , Name = $"{class2Name}s" });
                        classMap[class1Name].HasGenericList = true;
                    }
                    else if (!isClass1Collection) // One-to-one
                    {
                        classMap[class1Name].Attributes.Add(new AttributeInfo { Type = class2Name, Name = class2Name });
                    }
                }
            }
        }
        
        /// <summary>
        /// Extracts multiplicity information from class name string
        /// </summary>
        /// <param name="classNameWithMultiplicity">Class name string with possible multiplicity</param>
        /// <returns>Extracted multiplicity string</returns>
        private string ExtractMultiplicity(string classNameWithMultiplicity)
        {
            Match match = MultiplicityRegex.Match(classNameWithMultiplicity);
            return match.Success ? match.Value.Trim() : "1";
        }
        
        /// <summary>
        /// Determines if multiplicity represents a collection
        /// </summary>
        /// <param name="multiplicity">Multiplicity string</param>
        /// <returns>True if multiplicity indicates a collection</returns>
        private bool IsCollectionMultiplicity(string multiplicity)
        {
            return multiplicity.Contains("*") || multiplicity.Contains("..") || multiplicity == "0" || multiplicity == "n" || multiplicity == "N";
        }

        /// <summary>
        /// Maps Mermaid type names to C# type names
        /// </summary>
        /// <param name="mermaidType">Mermaid type name</param>
        /// <returns>Corresponding C# type name</returns>
        private string GetCSharpType(string mermaidType)
        {
            // Map common mermaid types to C# types
            Dictionary<string, string> typeMap = new Dictionary<string, string>
            {
                { "string", "string" },
                { "int", "int" },
                { "float", "float" },
                { "double", "double" },
                { "bool", "bool" },
                { "boolean", "bool" },
                { "void", "void" }
            };
            
            if (typeMap.ContainsKey(mermaidType.ToLower()))
            {
                return typeMap[mermaidType.ToLower()];
            }
            
            // If it's not a common type, assume it's a custom type
            return mermaidType;
        }

        /// <summary>
        /// Generates C# files from parsed class information
        /// </summary>
        /// <param name="classes">List of class information to generate</param>
        /// <param name="generatedFileDir">Directory to save generated files</param>
        private void GenerateCSharpFiles(List<ClassInfo> classes, string generatedFileDir)
        {
            // Validate directory path
            if (string.IsNullOrEmpty(generatedFileDir))
            {
                Debug.LogError("Generated file directory cannot be empty.");
                return;
            }

            // Create output directory if it doesn't exist
            if (!Directory.Exists(generatedFileDir))
            {
                Directory.CreateDirectory(generatedFileDir);
            }

            foreach (ClassInfo classInfo in classes)
            {
                // Preallocate capacity for StringBuilder for better performance
                StringBuilder sb = new StringBuilder(2000);

                // Generate file header
                sb.AppendLine("// This file is auto-generated from mermaid class diagram");
                sb.AppendLine("using UnityEngine;");
                if (classInfo.HasGenericList)
                {
                    sb.AppendLine("using System.Collections.Generic;");
                }
                sb.AppendLine();
                sb.AppendLine("namespace UAct.Generated");
                sb.AppendLine("{");

                // Generate class definition with inheritance
                if (string.IsNullOrEmpty(classInfo.BaseClassName))
                {
                    sb.AppendLine($"\tpublic class {classInfo.ClassName}");
                }
                else
                {
                    sb.AppendLine($"\tpublic class {classInfo.ClassName} : {classInfo.BaseClassName}");
                }
                sb.AppendLine("\t{");

                // Generate attributes
                if (classInfo.Attributes.Count > 0)
                {
                    foreach (AttributeInfo attribute in classInfo.Attributes)
                    {
                        if (string.IsNullOrEmpty(attribute.InitialValue))
                        {
                            sb.AppendLine($"\t\tpublic {attribute.Type} {attribute.Name};");
                        }
                        else
                        {
                            // Handle initialization values
                            string defaultValue = attribute.InitialValue;
                            sb.AppendLine($"\t\tpublic {attribute.Type} {attribute.Name} = {defaultValue};");
                        }
                    }
                    
                    if (classInfo.Methods.Count > 0)
                    {
                        sb.AppendLine();
                    }
                }

                // Generate methods
                foreach (MethodInfo method in classInfo.Methods)
                {
                    string paramsString = string.Join(", ", method.Parameters);
                    sb.AppendLine($"\t\tpublic {method.ReturnType} {method.Name}({paramsString})");
                    sb.AppendLine("\t\t{");
                    sb.AppendLine("\t\t\t// Method implementation goes here");
                    if (method.ReturnType != "void")
                    {
                        // Provide a default return value based on the return type
                        string defaultValue = GetDefaultValueForType(method.ReturnType);
                        sb.AppendLine($"\t\t\treturn {defaultValue};");
                    }
                    sb.AppendLine("\t\t}");
                }

                sb.AppendLine("\t}");
                sb.AppendLine("}");

                // Write to file using Path.Combine for cross-platform compatibility
                string fileName = $"{classInfo.ClassName}.cs";
                string filePath = Path.Combine(generatedFileDir, fileName);
                File.WriteAllText(filePath, sb.ToString());
            }
        }

        /// <summary>
        /// Gets the default value for a given C# type
        /// </summary>
        /// <param name="type">Type to get default value for</param>
        /// <returns>Default value string</returns>
        private string GetDefaultValueForType(string type)
        {
            if (string.IsNullOrEmpty(type)) return "null";
            
            if (type.StartsWith("List<"))
            {
                return $"new {type}()";
            }
            switch (type)
            {
                case "string":
                    return "\"\"";
                case "int":
                case "float":
                case "double":
                    return "0";
                case "bool":
                    return "false";
                default:
                    // For custom types, return null or default constructor
                    return type.Contains(" ") ? "null" : $"new {type}()";
            }
        }

        // Helper class to store class information
        private class ClassInfo
        {
            public string ClassName { get; set; }
            public string BaseClassName { get; set; } = null;
            public List<AttributeInfo> Attributes { get; set; }
            public List<MethodInfo> Methods { get; set; }
            public bool HasGenericList { get; set; } = false;

            public ClassInfo(string className)
            {
                ClassName = className;
                Attributes = new List<AttributeInfo>();
                Methods = new List<MethodInfo>();
            }
        }

        private class AttributeInfo
        {
            public string Type { get; set; }
            public string Name { get; set; }
            public string InitialValue { get; set; } = null;
        }

        private class MethodInfo
        {
            public string ReturnType { get; set; }
            public string Name { get; set; }
            public List<string> Parameters { get; set; }

            public MethodInfo()
            {
                Parameters = new List<string>();
            }
        }
    }
}