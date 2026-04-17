using UnityEngine;
using UnityEditor;

namespace UAct
{
    public abstract class EditorWindowBase<T> : EditorWindow where T : EditorWindowBase<T>
    {
        public const string MenuRoot = "UAct/";
        protected void CommandButton<U>(string text, ICommandContext context = null) where U : ICommand, new()
        {
            if (GUILayout.Button(text))
            {
                CommandCache.CallCommand<U>(context);
            }

        }

        protected void FlodOutPanel(string title, ref bool flodOut, System.Action action)
		{
			flodOut = EditorGUILayout.BeginFoldoutHeaderGroup(flodOut, title);
			if (flodOut) action();
			EditorGUILayout.EndFoldoutHeaderGroup();
		}

        public static void ShowWindow(string windowName)
		{
			EditorWindow window = GetWindow<T>();
			window.titleContent = new GUIContent(windowName);
			window.Show();
		}
        
    }
}