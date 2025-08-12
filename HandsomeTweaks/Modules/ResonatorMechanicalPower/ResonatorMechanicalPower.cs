using Vintagestory.API.Common;

using Jakojaannos.HandsomeTweaks.Modules.ResonatorMechanicalPower.GameContent;

namespace Jakojaannos.HandsomeTweaks.Modules.ResonatorMechanicalPower;

internal class ResonatorMechanicalPower : ModModule<HandsomeTweaksModSystem> {
	public const string MODULE_ID = "resonatormechanicalpower";
	public const string PATCH_CATEGORY = MODULE_ID;

	protected override string ModuleId => MODULE_ID;

	public override void Start(ICoreAPI api) {
		api.RegisterBlockClass(nameof(BlockMPResonator), typeof(BlockMPResonator));
	}
}
