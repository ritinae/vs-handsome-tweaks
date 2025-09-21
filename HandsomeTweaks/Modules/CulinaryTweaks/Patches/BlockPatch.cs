using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace Jakojaannos.HandsomeTweaks.Modules.CulinaryTweaks.Patches;

[HarmonyPatchCategory(CulinaryTweaks.PATCH_CATEGORY)]
[HarmonyPatch(typeof(Block))]
public static class BlockPatch {
	/// <summary>
	/// Attempts to make saucepan right-clickable in-world into the input slot
	/// on the firepit. This patch really targets only the `BlockFirepit`, but
	/// is implemented as a patch on `Block` to avoid need for a transpiler.
	///
	/// Thecode needs to run *after* vanilla `BlockFirepit` interaction logic,
	/// but before the `base.BlockInteractStart` occurs. Therefore, a simple
	/// postfix patch won't do, as it can't inject anything before the base
	/// method call. A transpiler would work, but is a lot more laborous to
	/// maintain than a simple postfix patch.
	///
	/// The trick here is to implement the patch as a prefix on the base method
	/// instead. A prefix is effectively the same as running the code before
	/// calling the base method. If the patched code succeeds at placing a
	/// saucepan into the fire pit, it early returns out.
	///
	/// The implementation itself mimics what vanilla does when right-clicking
	/// the firepit with a "meal container" in hand. Early returning out of the
	/// base method is effectively equivalent returning before calling the base
	/// method (apart from patches from other mods, but we'll worry about those
	/// when the need arises).
	///
	/// A cauldron is just a special kind of a saucepan, so this should work
	/// for those, too.
	/// </summary>
	[HarmonyPrefix]
	[HarmonyPatch(nameof(Block.OnBlockInteractStart))]
	public static bool OnBlockInteractStartPrefix(Block __instance, ref bool __result, IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel) {
		if (__instance is BlockFirepit @this && @this.Stage == 5) {
			var blockEntity = world.BlockAccessor.GetBlockEntity(blockSel.Position);
			var itemStack = byPlayer.InventoryManager.ActiveHotbarSlot?.Itemstack;
			var isSaucepan = itemStack?.Collectible.Class == "BlockSaucepan";
			if (blockEntity is BlockEntityFirepit beFirepit && isSaucepan) {
				// Try to put the saucepan to the input slot. The active hotbar
				// slot is guaranteed to be non-null, as itemstack was already
				// validated to be holding a saucepan.
				if (!beFirepit.inputSlot.Empty || byPlayer.InventoryManager.ActiveHotbarSlot!.TryPutInto(world, beFirepit.inputSlot) == 0) {
					beFirepit.OnPlayerRightClick(byPlayer, blockSel);
				}

				// Set the return value to `true` to eat up the interaction.
				// This matches vanilla behaviour of right-clicking the firepit
				// with a meal container.
				__result = true;
				return false;
			}
		}

		// Proceed with vanilla logic.
		return true;
	}
}
