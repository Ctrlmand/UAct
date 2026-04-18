using UnityEngine;
using UnityEditor;

namespace UAct.ContextMenu
{
	using Command.AssetsProcess;
	public class MaterialMenu
	{
		static BaseCommandContext cContext = new BaseCommandContext();
		[MenuItem("CONTEXT/Material/Rename by MainTex", false, 10)]
		public static void A (MenuCommand command)=> CommandManager.CallCommand<MainTexAsMatName>(cContext.SetData(command.context));
	}
}