using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace UAct.Util
{
	public static class Particle
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

		public static void SetCustomData(ParticleSystem ps)
		{
			ParticleSystemRenderer psr = ps.GetComponent<ParticleSystemRenderer>();
			var streams = new List<ParticleSystemVertexStream>(new ParticleSystemVertexStream[] { ParticleSystemVertexStream.Position, ParticleSystemVertexStream.Normal, ParticleSystemVertexStream.Color, ParticleSystemVertexStream.UV,
			ParticleSystemVertexStream.UV2,
			ParticleSystemVertexStream.Custom1XYZW
			});
			psr.SetActiveVertexStreams(streams);

			var cd = ps.customData;
			cd.enabled = true;
			cd.SetMode(ParticleSystemCustomData.Custom1, ParticleSystemCustomDataMode.Vector);

			Keyframe k00 = new Keyframe(0, 0);
			Keyframe k11 = new Keyframe(1, 1);
			k00.outWeight = 1;
			k11.inWeight = 1;

			AnimationCurve linearIncreaseCurve = AnimationCurve.Linear(0, 0, 1, 1);
			AnimationCurve constantCurve1 = AnimationCurve.Linear(0, 1, 1, 1);

			ParticleSystem.MinMaxCurve customX = new ParticleSystem.MinMaxCurve(1.0f, linearIncreaseCurve);
			ParticleSystem.MinMaxCurve customY = new ParticleSystem.MinMaxCurve(1.0f, constantCurve1);

			cd.SetVector(ParticleSystemCustomData.Custom1, 0, customX);
			cd.SetVector(ParticleSystemCustomData.Custom1, 1, customY);

		}

	}
}