using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.GameContent.Mechanics;

namespace Jakojaannos.HandsomeTweaks.Modules.ResonatorMechanicalPower.GameContent;

public class BEBehaviorMPConsumerWithDefaultRendering : BEBehaviorMPConsumer {
	public BEBehaviorMPConsumerWithDefaultRendering(BlockEntity blockentity) : base(blockentity) { }

	public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tesselator) {
		var _ = base.OnTesselation(mesher, tesselator);
		return false;
	}
}