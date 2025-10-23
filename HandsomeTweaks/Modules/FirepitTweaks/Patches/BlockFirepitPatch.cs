using System.Collections.Generic;

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
			var stacks = new List<ItemStack>() {
				new(api.World.GetBlock("game:claypot-blue-fired")),
				new(api.World.GetBlock("game:crucible-blue-fired")),
			};
			if (api.ModLoader.IsModSystemEnabled(typeof(CulinaryTweaks.CulinaryTweaks).FullName)) {
				var patterns = new AssetLocation[] {
					"aculinaryartillery:saucepan-*-fired",
					"aculinaryartillery:cauldron-*",
					"aculinaryartillery:cauldronmini-*"
				};

				foreach (var pattern in patterns) {
					var matches = api.World.SearchBlocks(pattern);
					foreach (var insertableBlock in matches) {
						stacks.Add(new ItemStack(insertableBlock));
					}
				}
			}

			return stacks.ToArray();
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
