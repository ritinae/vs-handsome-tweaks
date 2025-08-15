using HarmonyLib;

using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace Jakojaannos.HandsomeTweaks.Modules.FirepitTweaks.Patches;

[HarmonyPatchCategory(FirepitTweaks.PATCH_CATEGORY)]
[HarmonyPatch(typeof(BlockFirepit))]
public static class BlockFirepitPatch {
	[HarmonyPostfix]
	[HarmonyPatch(nameof(BlockFirepit.OnLoaded))]
	public static void OnLoadedPostfix(BlockFirepit __instance, ref WorldInteraction[] ___interactions, ICoreAPI api) {
		var insertableStacks = ObjectCacheUtil.GetOrCreate(api, "handsometweaks:firepittweaks-firepitInsertableStacks", () => {
			var stacks = new ItemStack[] {
				new(api.World.GetBlock("game:claypot-burned")),
				new(api.World.GetBlock("game:crucible-burned")),
			};
			if (api.ModLoader.IsModSystemEnabled(typeof(CulinaryTweaks.CulinaryTweaks).FullName)) {
				stacks = stacks.Append(
					new ItemStack(api.World.GetBlock("aculinaryartillery:saucepan-burned"))
				);

				foreach (var block in api.World.Blocks) {
					if (block.Code.BeginsWith("aculinaryartillery", "cauldron")) {
						stacks = stacks.Append(new ItemStack(block));
					}
				}
			}

			return stacks;
		});

		___interactions = ObjectCacheUtil.GetOrCreate(
			api,
			"handsometweaks:firepitInteractions-" + __instance.Stage,
			() => {
				var interactions = api.ObjectCache["firepitInteractions-" + __instance.Stage] as WorldInteraction[];
				return interactions
					.Append(new WorldInteraction() {
						ActionLangCode = "handsometweaks:blockhelp-firepit-insert",
						MouseButton = EnumMouseButton.Right,
						Itemstacks = insertableStacks,
						ShouldApply = (_, _, _) => __instance.Stage == 5,
					});

			});
	}
}
