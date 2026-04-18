using UnityEditor;
using UnityEngine;

namespace UAct.ItemMenu
{
	using Command.EffectOperation;

	public class ParticleSystemMenu : MonoBehaviour
	{
		[MenuItem("CONTEXT/ParticleSystem/Make Material Single User", false, 10)]
		public static void MakeMaterialSingleUser()=> CommandManager.CallCommand<MakeMaterialSingleUser>();


	}
}
