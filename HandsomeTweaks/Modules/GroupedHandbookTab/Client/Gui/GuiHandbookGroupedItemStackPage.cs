using System.Collections.Generic;

using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace Jakojaannos.HandsomeTweaks.Modules.GroupedHandbookTab.Client.Gui;

internal class GuiHandbookGroupedItemPage : GuiHandbookItemStackPage {
	public List<ItemStack> Stacks { get; } = new List<ItemStack>();
	public string Name { get; }
	public override string PageCode => Name;
	public override string CategoryCode => "handsometweaks:grouped";

	public GuiHandbookGroupedItemPage(ICoreClientAPI capi, IEnumerable<ItemStack> items, string name)
		// Any itemstack can be passed to the parent. It just needs to be a
		// valid non-null item, for the constructor to pass without errors.
		// FIXME: remove the inheritance as we don't really use base class
		//        features to begin with.
		: base(capi, new ItemStack(capi.World.Items[0])) {
		Stacks.AddRange(items);

		Name = name;
	}

	public override void RenderListEntryTo(ICoreClientAPI capi, float dt, double x, double y, double cellWidth, double cellHeight) {
		var size = (float)GuiElement.scaled(25.0);
		var offsetX = (float)GuiElement.scaled(10.0);

		var currentItemIndex = (int)(capi.ElapsedMilliseconds / 1000 % Stacks.Count);
		dummySlot.Itemstack = Stacks[currentItemIndex];
		capi.Render.RenderItemstackToGui(
			dummySlot,
			posX: x + (double)offsetX + (double)(size / 2f),
			posY: y + (double)(size / 2f),
			posZ: 100.0,
			size,
			color: -1,
			shading: true,
			rotate: false,
			showStackSize: false
		);
		Texture ??= new TextTextureUtil(capi).GenTextTexture(Name, CairoFont.WhiteSmallText());
		capi.Render.Render2DTexturePremultipliedAlpha(Texture.TextureId, x + (double)size + GuiElement.scaled(25.0), y + (double)(size / 4f) - 3.0, Texture.Width, Texture.Height);
	}

	protected override RichTextComponentBase[] GetPageText(ICoreClientAPI capi, ItemStack[] allStacks, ActionConsumable<string> openDetailPageFor) {
		dummySlot.Itemstack = Stacks[0];
		var vanillaContent = Stacks[0].Collectible.GetBehavior<CollectibleBehaviorHandbookTextAndExtraInfo>();
		var wrapper = new HandbookTextBehaviorWrapper(vanillaContent.collObj, Stacks.ToArray(), Name);
		return wrapper.GetHandbookInfo(dummySlot, capi, allStacks, openDetailPageFor);
	}
}
