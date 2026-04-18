using System.IO;
using UnityEditor;
using UnityEngine;

namespace UAct.Command.EffectOperation
{

	public class MakeMaterialSingleUser : ICommand
	{
		public void Execute(ICommandContext context)
		{
			GameObject active = Selection.activeGameObject;
			if (active==null) return;

			// try get ParticleSystem
			if (active.TryGetComponent(out ParticleSystem particleSystem))
			{
				DuplicateReplaceMatForParticleSystem(particleSystem);
				return;
			}

		}

		private void DuplicateReplaceMatForParticleSystem(ParticleSystem particleSystem)
		{
			ParticleSystemRenderer renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
			if (renderer == null) return;

			Material currentMat = renderer.sharedMaterial;
			if (currentMat == null) return;

			string assetPath = AssetDatabase.GetAssetPath(currentMat);
			// handle Packages Assets
			if (assetPath.StartsWith("Packages"))
			{
				Debug.LogWarning("target material is Packages Assets");
				return;
			}
			
			string extension = Path.GetExtension(assetPath);
			string newPath = assetPath.Replace(extension, $"_Copy{extension}");
			AssetDatabase.CopyAsset(assetPath, newPath);

			Material newMat = AssetDatabase.LoadAssetAtPath<Material>(newPath);
			renderer.sharedMaterial = newMat;

			Undo.RegisterCreatedObjectUndo(newMat, "Create");
			Selection.activeObject = newMat;
			EditorGUIUtility.PingObject(newMat);
		}
	}
}