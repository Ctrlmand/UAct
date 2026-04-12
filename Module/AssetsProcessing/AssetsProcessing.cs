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
		bool useConfigFile = false;
		
		string newName = "";
		string newNameSuffix = "";


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
			GUILayout.Label("Model", EditorStyles.boldLabel);
			shader = EditorGUILayout.ObjectField("Material Shader", shader, typeof(Shader), false) as Shader;
			CommandButton<ExtractMaterial>("Extract Material", new BaseCommandContext().SetData(shader));
			// CommandButton<ExtractMaterialCommand>("Extract Material", new BaseCommandContext(shader));
			remapMatDirectory = EditorGUILayout.ObjectField("Remap Mat Folder", remapMatDirectory, typeof(Object), false);
			CommandButton<BatchRemapMat>("Remap Material", new BaseCommandContext().SetData(AssetDatabase.GetAssetPath(remapMatDirectory)));
			
			// Material Tools
			GUILayout.Space(15);
			GUILayout.Label("Material", EditorStyles.boldLabel);
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
				if (GUILayout.Button("Store Preset")) AssignTexture.StorePreset(mapInfo);
				if (GUILayout.Button("Generate By Selected Material")) AssignTexture.GenerateMapConfigBySelected(ref mapInfo);
				GUILayout.EndHorizontal();

			}
			CommandButton<AssignTexture>("Assign Texture", new AssignTextureContext(useConfigFile, configFile, new string[] {mapInfo, matPrefix, texPrefix}));
			matPrefix = EditorGUILayout.TextField("Material Prefix", matPrefix);
			texPrefix = EditorGUILayout.TextField("Texture Prefix", texPrefix);
			CommandButton<MainTexAsMatName>("Main Tex As MatName");
			
			// Prefab
			GUILayout.Space(15);
			GUILayout.Label("Prefab", EditorStyles.boldLabel);
			CommandButton<FbxToPrefabs>("FBX to Prefabs");

			// Texture Tools
			GUILayout.Space(10);
			GUILayout.Label("Texture", EditorStyles.boldLabel);
			CommandButton<InvertGChannel>("Invert G Channel");

			GUILayout.Space(10);
			GUILayout.Label("资产重命名", EditorStyles.boldLabel);
			newName = EditorGUILayout.TextField("新名称", newName);
			newNameSuffix = EditorGUILayout.TextField("后缀", newNameSuffix);

			CommandButton<RenameAssets>("重命名", new BaseCommandContext().SetData(new string[] { newName, newNameSuffix }));
		}

	}
	
}