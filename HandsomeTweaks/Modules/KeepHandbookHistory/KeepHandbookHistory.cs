using Vintagestory.API.Common;

namespace Jakojaannos.HandsomeTweaks.Modules.KeepHandbookHistory;

internal class KeepHandbookHistory : ModModule<HandsomeTweaksModSystem> {
	public const string MODULE_ID = "keephandbookhistory";
	public const string PATCH_CATEGORY = MODULE_ID;

	protected override string ModuleId => MODULE_ID;

	public override bool ShouldLoad(EnumAppSide forSide) {
		return forSide == EnumAppSide.Client;
	}
}
