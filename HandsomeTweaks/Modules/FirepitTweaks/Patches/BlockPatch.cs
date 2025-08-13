using HarmonyLib;

using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace Jakojaannos.HandsomeTweaks.Modules.FirepitTweaks.Patches;

[HarmonyPatchCategory(FirepitTweaks.PATCH_CATEGORY)]
[HarmonyPatch(typeof(Block))]
public static class BlockPatch {
	/// <summary>
	/// The same thing as saucepan tweak, in CulinaryTweaks module, but for
	/// crucible-like stuff. How the patch work and why it targets `Block`
	/// instead of `BlockFirepit` is documented in that module.
	/// </summary>
	[HarmonyPrefix]
	[HarmonyPatch(nameof(Block.OnBlockInteractStart))]
	public static bool OnBlockInteractStartPrefix(Block __instance, ref bool __result, IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel) {
		if (__instance is BlockFirepit @this && @this.Stage == 5) {
			var blockEntity = world.BlockAccessor.GetBlockEntity(blockSel.Position);
			var itemStack = byPlayer.InventoryManager.ActiveHotbarSlot?.Itemstack;
			if (blockEntity is BlockEntityFirepit beFirepit && itemStack?.Collectible is BlockSmeltingContainer) {
				if (!beFirepit.inputSlot.Empty || byPlayer.InventoryManager.ActiveHotbarSlot!.TryPutInto(world, beFirepit.inputSlot) == 0) {
					beFirepit.OnPlayerRightClick(byPlayer, blockSel);
				}

				__result = true;
				return false;
			}
		}

		return true;
	}
}
