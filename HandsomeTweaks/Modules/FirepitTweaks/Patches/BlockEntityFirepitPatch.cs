using HarmonyLib;

using Vintagestory.GameContent;

namespace Jakojaannos.HandsomeTweaks.Modules.FirepitTweaks.Patches;

[HarmonyPatchCategory(FirepitTweaks.PATCH_CATEGORY)]
[HarmonyPatch(typeof(BlockEntityFirepit))]
public static class BlockEntityFirepitPatch {
	private const int OUTPUT_SLOT = 2;

	/// <summary>
	/// Attempts to swap empty cooking pots from firepit output slot to the
	/// input slot automagically. This occurs e.g. when taking out food from a
	/// pot with a crock via an inventory interaction.
	///
	/// Goal is to reduce unnecessary clicking in the interface.
	///
	/// The patch works by performing a check every time the output slot of a
	/// firepit is modified. If the output slot contains a cooking pot and if
	/// the input slot is empty, the slots can be safely swapped.
	/// </summary>
	[HarmonyPostfix]
	[HarmonyPatch(MethodType.Constructor)]
	public static void ConstructorPostfix(BlockEntityFirepit __instance) {
		__instance.Inventory.SlotModified += (slotIndex) => {
			var outputSlot = __instance.outputSlot;
			if (slotIndex != OUTPUT_SLOT || outputSlot.Empty) {
				return;
			}

			var inputSlot = __instance.inputSlot;
			var outputStack = __instance.outputStack;

			var isCookingContainerInOutput = outputStack.ItemAttributes?.KeyExists("cookingContainerSlots") == true;
			if (isCookingContainerInOutput && inputSlot.Empty) {
				if (outputSlot.TryFlipWith(inputSlot)) {
					inputSlot.MarkDirty();
					outputSlot.MarkDirty();
				}
			}
		};
	}
}
