using UnityEditor;
using UnityEngine;

namespace UAct.AssetsProcessing
{
	public class AssetsProcessing : EditorWindowBase<AssetsProcessing>
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

		[MenuItem(MenuRoot+nameof(AssetsProcessing), false, 0)]
		public static void Open() => ShowWindow(nameof(AssetsProcessing));

		bool modelFlodOut=true, prefabFlodOut=true, materialFlodOut=true, textureFlodOut=true, renameFlodOut=true;
		void OnGUI()
		{

			FlodOutPanel("Model", ref modelFlodOut, ModelGUI);

			FlodOutPanel("Material", ref materialFlodOut, MaterialGUI);

			FlodOutPanel("Prefab", ref prefabFlodOut, PrefabGUI);

			FlodOutPanel("Texture", ref textureFlodOut, TextureGUI);

			FlodOutPanel("Rename", ref renameFlodOut, RenameGUI);

		}

		void ModelGUI()
		{
			shader = EditorGUILayout.ObjectField("Material Shader", shader, typeof(Shader), false) as Shader;
			CommandButton<ExtractMaterial>("Extract Material", new BaseCommandContext().SetData(shader));
			// CommandButton<ExtractMaterialCommand>("Extract Material", new BaseCommandContext(shader));
			remapMatDirectory = EditorGUILayout.ObjectField("Remap Mat Folder", remapMatDirectory, typeof(Object), false);
			CommandButton<BatchRemapMat>("Remap Material", new BaseCommandContext().SetData(AssetDatabase.GetAssetPath(remapMatDirectory)));

		}

		void MaterialGUI()
		{
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

		}

		void PrefabGUI()
		{
			CommandButton<FbxToPrefabs>("FBX to Prefabs");
			EditorGUILayout.EndFoldoutHeaderGroup();

		}

		void TextureGUI()
		{
			CommandButton<InvertGChannel>("Invert G Channel");

		}

		void RenameGUI()
		{
			newName = EditorGUILayout.TextField("新名称", newName);
			newNameSuffix = EditorGUILayout.TextField("后缀", newNameSuffix);
			CommandButton<RenameAssets>("重命名", new BaseCommandContext().SetData(new string[] { newName, newNameSuffix }));

		}
	}
	
}