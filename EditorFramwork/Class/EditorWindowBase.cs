using UnityEngine;
using UnityEditor;

namespace UAct
{

    public abstract class EditorWindowBase : EditorWindow
    {
        protected void CommandButton<T>(string text, ICommandContext context = null) where T : ICommand, new()
        {
            if (GUILayout.Button(text))
            {
                CommandCache.GetCommand<T>().Execute(context);
            }

        }
    }
}