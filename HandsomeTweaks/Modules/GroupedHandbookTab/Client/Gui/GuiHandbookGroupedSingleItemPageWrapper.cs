using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace Jakojaannos.HandsomeTweaks.Modules.GroupedHandbookTab.Client.Gui;

internal class GuiHandbookGroupedSingleItemPageWrapper : GuiHandbookItemStackPage {
	public override string CategoryCode => "handsometweaks:grouped";

	public GuiHandbookGroupedSingleItemPageWrapper(ICoreClientAPI capi, ItemStack stack) : base(capi, stack) {
	}
}
