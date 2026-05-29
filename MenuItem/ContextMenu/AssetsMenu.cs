using System;
using UnityEditor;
using UnityEngine;

namespace UAct.ContextMenu
{
	using Util;
	public class AssetsMenu : ContextMenuBase
	{
		private const int menuItemIdCounter = 20;

		[MenuItem("GameObject/Effects/Empty Particle System", false, menuItemIdCounter)]
		public static void CreateEmptyParticleSystem (MenuCommand command)=>
			CreateGameobject("Empty Particle System", command.context as GameObject, SetEmptyParticleSystem);

		[MenuItem("GameObject/Effects/Static Particle System", false, menuItemIdCounter)]
		public static void CreateStaticParticleSystem(MenuCommand command) =>
			CreateGameobject("Static Particle System", command.context as GameObject, SetStaticParticleSystem);

		[MenuItem("GameObject/Effects/Static Mesh Particle System", false, menuItemIdCounter)]
		public static void CreateStaticMeshParticleSystem(MenuCommand command) =>
			CreateGameobject("Static Mesh Particle System", command.context as GameObject, SetStaticMeshParticleSystem);
		
		private static void SetEmptyParticleSystem(GameObject go)
		{
			ParticleSystem ps = InitParticleSystem(go);

			var em = ps.emission;
			em.enabled = false;

			var sh = ps.shape;
			sh.shapeType = ParticleSystemShapeType.Sphere;

		}

		private static void SetStaticMeshParticleSystem(GameObject go)
		{
			ParticleSystem ps = InitParticleSystem(go);

			
			ParticleSystemRenderer psr = go.GetComponent<ParticleSystemRenderer>();
			psr.renderMode = ParticleSystemRenderMode.Mesh;
			psr.mesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
			psr.alignment = ParticleSystemRenderSpace.Local;
			psr.material = Particle.GetDefaultParticleMaterial();

			Particle.SetCustomData(ps);
    	}

		private static void SetStaticParticleSystem(GameObject go)
		{
			ParticleSystem ps = InitParticleSystem(go);

			ParticleSystemRenderer psr = go.GetComponent<ParticleSystemRenderer>();
			psr.material = Particle.GetDefaultParticleMaterial();

		}

		private static ParticleSystem InitParticleSystem(GameObject go)
		{
			ParticleSystem ps = go.AddComponent<ParticleSystem>();

			var main = ps.main;
			main.loop = false;
			main.startSpeed = 0f;
			main.startLifetime = 1f;
			main.scalingMode = ParticleSystemScalingMode.Hierarchy;

			var em = ps.emission;
			em.rateOverTime = 0;
			em.burstCount = 1;
			em.SetBurst(0, new ParticleSystem.Burst(0.0f, 1));

			var sh = ps.shape;
			sh.shapeType = ParticleSystemShapeType.Sphere;
			sh.enabled = false;

			return ps;
		}
					
	}
}