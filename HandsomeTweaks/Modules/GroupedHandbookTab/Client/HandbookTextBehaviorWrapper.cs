using System.Collections.Generic;

using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace Jakojaannos.HandsomeTweaks.Modules.GroupedHandbookTab.Client;

// FIXME: This likely doesn't need to be a wrapper after all
internal class HandbookTextBehaviorWrapper : CollectibleBehaviorHandbookTextAndExtraInfo {
	private ItemStack[] VariantItemStacks { get; }
	private string GroupName { get; }

	public HandbookTextBehaviorWrapper(CollectibleObject collObj, ItemStack[] variants, string name) : base(collObj) {
		VariantItemStacks = variants;
		GroupName = name;
	}

	public override RichTextComponentBase[] GetHandbookInfo(ItemSlot inSlot, ICoreClientAPI capi, ItemStack[] allStacks, ActionConsumable<string> openDetailPageFor) {
		var components = new List<RichTextComponentBase>();

		AddVariantAwareGeneralInfo(capi, components);

		var haveText = components.Count > 0;
		AddVariantInfo(capi, components, openDetailPageFor, ref haveText);

		return components.ToArray();
	}

	protected void AddVariantAwareGeneralInfo(ICoreClientAPI capi, List<RichTextComponentBase> components) {
		var slideshow = new SlideshowItemstackTextComponent(capi, VariantItemStacks, 100.0, EnumFloat.Left) {
			PaddingRight = 10.0
		};
		components.Add(slideshow);
		components.AddRange(VtmlUtil.Richtextify(capi, GroupName + "\n", CairoFont.WhiteSmallishText()));
	}

	protected void AddVariantInfo(ICoreClientAPI capi, List<RichTextComponentBase> components, ActionConsumable<string> openDetailPageFor, ref bool haveText) {
		AddHeading(components, capi, "Variants", ref haveText);
		components.Add(new ClearFloatTextComponent(capi, 2f));
		foreach (var variant in VariantItemStacks) {
			var item = new ItemstackTextComponent(
				capi,
				variant,
				40.0,
				floatType: EnumFloat.Inline,
				onStackClicked: cs => openDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(cs))
			);
			components.Add(item);
		}
		components.Add(new ClearFloatTextComponent(capi, 3f));
	}
}
