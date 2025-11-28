using UnityEditor;
using UnityEngine;

namespace UAct.AssetsProcessing
{
	public class AutoAssignTextures : EditorWindowBase
	{
		Shader shader;
		Object remapMatDirectory;
		Object configFile;
		string mapInfo = "BaseMap => _BaseMap\nMetallic => _MetallicGlossMap\nNormal => _BumpMap\nHeight => _ParallaxMap\nAO => _OcclusionMap\nEmission => _EmissionMap";
		string matPrefix = "";
		string texPrefix = "";
		bool useConfigFile = true;
		

		[MenuItem("UAct/AssetsProcessing", false, 0)]
		public static void ShowWindow()
		{
			var window = GetWindow<AutoAssignTextures>();
			window.titleContent = new GUIContent("AssetsProcessing");
			window.Show();
		}

		void OnGUI()
		{
			// Model Tools
			GUILayout.Label("Model Tool", EditorStyles.boldLabel);
			shader = EditorGUILayout.ObjectField("Material Shader", shader, typeof(Shader), false) as Shader;
			CommandButton<ExtractMaterialCommand>("Extract Material", new BaseCommandContext().SetData(shader));
			// CommandButton<ExtractMaterialCommand>("Extract Material", new BaseCommandContext(shader));
			remapMatDirectory = EditorGUILayout.ObjectField("Remap Mat Folder", remapMatDirectory, typeof(Object), false);
			CommandButton<BatchRemapMatCommand>("Remap Material", new BaseCommandContext().SetData(AssetDatabase.GetAssetPath(remapMatDirectory)));
			
			// Material Tools
			GUILayout.Space(15);
			GUILayout.Label("Material Tools", EditorStyles.boldLabel);
			// Use config file or not
			useConfigFile = EditorGUILayout.Toggle("Use Config File", useConfigFile);
			if (useConfigFile)
			{
				// Config File
				configFile = EditorGUILayout.ObjectField("Config File", configFile, typeof(TextAsset), false);
			}
			else
			{
				// Map Info
				GUILayout.Label("{Texture suffix} => {Shader properties}\nMeans assign a Prefix_{Texture suffix} tex to _{Shader properties}", EditorStyles.helpBox);
				mapInfo = EditorGUILayout.TextArea(mapInfo, GUILayout.Height(100), GUILayout.ExpandWidth(true));
				
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Store Preset")) AssignTextureCommand.StorePreset(mapInfo);
				if (GUILayout.Button("Generate By Selected Material")) AssignTextureCommand.GenerateMapConfigBySelected(ref mapInfo);
				GUILayout.EndHorizontal();

			}
			matPrefix = EditorGUILayout.TextField("Material Prefix", matPrefix);
			texPrefix = EditorGUILayout.TextField("Texture Prefix", texPrefix);
			CommandButton<AssignTextureCommand>("Assign Texture", new AssignTextureContext(useConfigFile, configFile, new string[] {mapInfo, matPrefix, texPrefix}));
			
			// Prefab
			GUILayout.Space(15);
			GUILayout.Label("Prefab", EditorStyles.boldLabel);
			CommandButton<FbxToPrefabs>("FBX to Prefabs");

			// Texture Tools
			GUILayout.Space(10);
			GUILayout.Label("Texture Tools", EditorStyles.boldLabel);
			CommandButton<InvertGChannelCommand>("Invert G Channel");

		}

	}
	
}