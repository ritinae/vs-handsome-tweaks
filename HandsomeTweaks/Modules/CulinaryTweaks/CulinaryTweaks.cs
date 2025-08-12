using Vintagestory.API.Common;

using Jakojaannos.HandsomeTweaks.Compatibility;

namespace Jakojaannos.HandsomeTweaks.Modules.CulinaryTweaks;

internal class CulinaryTweaks : ModModule<HandsomeTweaksModSystem> {
	public const string MODULE_ID = "culinarytweaks";
	public const string PATCH_CATEGORY = MODULE_ID;

	protected override string ModuleId => MODULE_ID;

	public override bool ShouldLoad(ICoreAPI api) {
		return base.ShouldLoad(api) && api.ModLoader.IsModEnabled(ModIds.A_CULINARY_ARTILLERY);
	}
}
