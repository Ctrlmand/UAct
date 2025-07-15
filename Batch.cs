using UnityEditor;
using UnityEngine;
using Extension.Editor.Framwork;


namespace Extension.Editor.Tools.Batch
{

    public class AutoAssignTextures : EditorWindowBase
    {
        [MenuItem("UAct/Batch")]
        public static void ShowWindow()
        {
            var window = GetWindow<AutoAssignTextures>();
            window.titleContent = new GUIContent("Batch");
            window.Show();
        }
        void OnGUI()
        {
            GUILayout.Label("Material Tools", EditorStyles.boldLabel);
            NewButton<AssignTextureCommand>("Assign Texture");

            GUILayout.Label("Texture Tools", EditorStyles.boldLabel);
            NewButton<InvertGChannelCommand>("Invert G Channel");

            GUILayout.Label("Model Tool", EditorStyles.boldLabel);
            NewButton<ExtractMaterialCommand>("Extract Material");

            GUILayout.Label("Get Info", EditorStyles.boldLabel);
            if (GUILayout.Button("Get Path")) UtilFunc.Assets.GetSelectedAssetPath();
            if (GUILayout.Button("Get Obj Type")) UtilFunc.Assets.GetSelectAssetType();

        }

    }
    
}