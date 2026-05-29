using UnityEngine;
using UnityEditor;

namespace UAct
{
	public class ContextMenuBase
	{
		/// <summary>
		/// create a gameobject with given name, parent and process it with custom action, auto register undo and set active.
		/// </summary>
		/// <param name="name">gameobject name</param>
		/// <param name="parent">parent transform</param>
		/// <param name="customAction">custom action to process the gameobject</param>
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
