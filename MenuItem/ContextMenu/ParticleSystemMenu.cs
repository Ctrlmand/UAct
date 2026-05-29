using UnityEditor;
using UnityEngine;

namespace UAct.ItemMenu
{
	using Command.EffectOperation;
	using Util;
	public class ParticleSystemMenu : MonoBehaviour
	{
		[MenuItem("CONTEXT/ParticleSystem/Make Material Single User", false, 10)]
		public static void MakeMaterialSingleUser()=> CommandManager.CallCommand<MakeMaterialSingleUser>();
		[MenuItem("CONTEXT/ParticleSystem/Set Custom Data", false, 11)]
		public static void SetParticleSystemCustomData(MenuCommand command) =>
		Particle.SetCustomData(command.context as ParticleSystem);


	}
}
