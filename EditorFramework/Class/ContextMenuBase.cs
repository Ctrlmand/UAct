using UnityEngine;
using UnityEditor;

namespace UAct
{
	public class ContextMenuBase
	{
		public static void CreateGameobject(string name, GameObject parent, System.Action<GameObject> customAction)
		{
			// create go, redirect parent level
			GameObject go = new GameObject(name);
			GameObjectUtility.SetParentAndAlign(go, parent);

			// invoke action
			customAction?.Invoke(go);

			// register undo, set go active
			Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
			Selection.activeGameObject = go;
		}

	}
}
