using UnityEditor;
using UnityEngine;

namespace UAct.Batch
{
    public class AutoAssignTextures : EditorWindowBase
    {
        Shader shader;
        Object configFile;
        string mapInfo = "BaseMap => _BaseMap\nMetallic => _MetallicGlossMap\nNormal => _BumpMap\nHeight => _ParallaxMap\nAO => _OcclusionMa\nEmission => _EmissionMap";
        bool isUseConfigFile = true;
        

        [MenuItem("UAct/Batch", false, 0)]
        public static void ShowWindow()
        {
            var window = GetWindow<AutoAssignTextures>();
            window.titleContent = new GUIContent("Batch");
            window.Show();
        }

        void OnGUI()
        {
            GUILayout.Label("Model Tool", EditorStyles.boldLabel);
            shader = EditorGUILayout.ObjectField("Material Shader", shader, typeof(Shader), false) as Shader;
            CommandButton<ExtractMaterialCommand>("Extract Material", new BaseCommandContext(shader));
            
			GUILayout.Space(10);
            GUILayout.Label("Material Tools", EditorStyles.boldLabel);
            // Use config file or not
            isUseConfigFile = EditorGUILayout.Toggle("Use Config File", isUseConfigFile);
            if (isUseConfigFile)
            {
                configFile = EditorGUILayout.ObjectField("Config File", configFile, typeof(Object), false);
            }
            else
            {
                GUILayout.Label("{Texture suffix} => {Shader properties}\nMeans assign a Prefix_{Texture suffix} tex to _{Shader properties}", EditorStyles.helpBox);
                mapInfo = EditorGUILayout.TextArea(mapInfo, GUILayout.Height(100), GUILayout.ExpandWidth(true));
                if (GUILayout.Button("Store Preset", EditorStyles.toolbarButton)) AssignTextureCommand.StorePreset(mapInfo);
                
            }
            CommandButton<AssignTextureCommand>("Assign Texture", new AssignTextureContext(isUseConfigFile, configFile, mapInfo));

			GUILayout.Space(10);
            GUILayout.Label("Texture Tools", EditorStyles.boldLabel);
            CommandButton<InvertGChannelCommand>("Invert G Channel");

        }

    }
    
}