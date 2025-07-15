using UnityEngine;
using UnityEditor;
using Extension.Editor.Framwork;

namespace Extension.Editor
{

    public class EditorWindowBase : EditorWindow
    {
        protected void NewButton<T>(string text) where T : ICommand, new()
        {
            if (GUILayout.Button(text)) CommandCache.GetCommand<T>().Execute();
        }
    }
}