using HarmonyLib;

using Jakojaannos.HandsomeTweaks.Config;
using Jakojaannos.HandsomeTweaks.Modules.MergeStacksOnGround.Patches;

namespace Jakojaannos.HandsomeTweaks.Modules.MergeStacksOnGround;

internal class MergeStacksOnGround : ModModule<HandsomeTweaksModSystem> {
	public const string MODULE_ID = "mergestacksonground";
	public const string PATCH_CATEGORY = MODULE_ID;

	protected override string ModuleId => MODULE_ID;

	public static HandsomeTweaksSettings.MergeStacksOnGroundSettings Settings => HandsomeTweaksSettings.Instance.MergeStacksOnGround;

	protected override void ApplyPatches(Harmony harmony) {
		base.ApplyPatches(harmony);

		if (Settings.IsRenderPatchEnabled) {
			harmony.PatchCategory(PATCH_CATEGORY + EntityItemRendererPatch.RENDER_PATCH);
		}
	}
}
