using System.Collections.Generic;
using System.Linq;

using HarmonyLib;

using Jakojaannos.HandsomeTweaks.Modules.GroupedHandbookTab.Client.Gui;

using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Util;
using Vintagestory.GameContent;
using Vintagestory.ServerMods.NoObf;

namespace Jakojaannos.HandsomeTweaks.Modules.GroupedHandbookTab.Patches;

[HarmonyPatchCategory(GroupedHandbookTab.PATCH_CATEGORY)]
[HarmonyPatch(typeof(GuiDialogHandbook))]
public static class GuiDialogHandbookPatch {
	[HarmonyPrepare]
	public static bool IsEnabled() {
		return GroupedHandbookTab.IsEnabled;
	}

	/// <summary>
	/// Remove grouped pages from the "Everything" tab.
	/// </summary>
	[HarmonyPostfix]
	[HarmonyPatch(nameof(GuiDialogHandbook.FilterItems))]
	public static void FilterItemsPostfix(
		GuiDialogHandbook __instance,
		bool ___loadingPagesAsync,
		GuiComposer ___overviewGui,
		double ___listHeight,
		ref List<IFlatListItem> ___shownHandbookPages
	) {
		// Don't do anything if still loading or if not on the "Everything" tab
		if (___loadingPagesAsync || __instance.currentCatgoryCode != null) {
			return;
		}

		___shownHandbookPages.RemoveAll(shown => shown is GuiHandbookPage page && page.CategoryCode == "handsometweaks:grouped");

		var flatList = ___overviewGui.GetFlatList("stacklist");
		flatList.CalcTotalHeight();
		___overviewGui.GetScrollbar("scrollbar").SetHeights((float)___listHeight, (float)flatList.insideBounds.fixedHeight);
	}
}
