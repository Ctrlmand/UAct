using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace UAct.Util
{
	public static class LoadDefault
	{
		public static string GetCurrentRenderPipelineName()
		{
			RenderPipelineAsset pipelineAsset = GraphicsSettings.currentRenderPipeline;
            return pipelineAsset.GetType().Name;
		}
		public static Material GetDefaultParticleMaterial()
		{
			string pipelineName = GetCurrentRenderPipelineName();

			if (pipelineName.Contains("Universal"))
			{
				return GraphicsSettings.currentRenderPipeline.defaultParticleMaterial;
			}

			return Resources.GetBuiltinResource<Material>("Default-Particle.mat");
		}

	}
}