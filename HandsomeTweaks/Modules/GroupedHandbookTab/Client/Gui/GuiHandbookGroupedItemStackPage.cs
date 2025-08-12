using System;
using System.Collections.Generic;
using System.Linq;

using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace Jakojaannos.HandsomeTweaks.Modules.GroupedHandbookTab.Client.Gui;

internal class GuiHandbookGroupedItemPage : GuiHandbookItemStackPage {
	public List<ItemStack> Stacks { get; } = new List<ItemStack>();
	public string Name { get; }
	public override string PageCode => Name;
	public override string CategoryCode => "handsometweaks:grouped";

	private readonly struct TextCache {
		public required string Title { get; init; }
		public required string All { get; init; }
	}

	private readonly Dictionary<AssetLocation, TextCache> _stackTextCache;

	public GuiHandbookGroupedItemPage(ICoreClientAPI capi, IEnumerable<ItemStack> items, string name)
		// Any itemstack can be passed to the parent. It just needs to be a
		// valid non-null item, for the constructor to pass without errors.
		// FIXME: remove the inheritance as we don't really use base class
		//        features to begin with.
		: base(capi, new ItemStack(capi.World.Items[0])) {
		Stacks.AddRange(items);

		TextCacheTitle = name.ToSearchFriendly();
		TextCacheAll = $"Group: {name}";

		_stackTextCache = new();
		foreach (var stack in Stacks) {
			if (_stackTextCache.ContainsKey(stack.Collectible.Code)) {
				// FIXME: e.g. shields have funky variants with multiple items with the same code and id
				continue;
			}
			var searchTitle = stack.GetName().ToSearchFriendly();
			// FIXME: some ItemWearable references null when getting descriptions
			var searchFullText = stack.GetName();// + " " + stack.GetDescription(capi.World, dummySlot);

			_stackTextCache.Add(stack.Collectible.Code, new() {
				Title = searchTitle,
				All = searchFullText,
			});
		}

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

	public override float GetTextMatchWeight(string searchText) {
		var mainTitleOffset = 0.25f;
		var groupMemberOffset = -0.125f;
		var group = GetOffsetTextMatchWeight(TextCacheTitle, TextCacheAll, searchText, searchWeightOffset + mainTitleOffset);
		var stacks = Stacks.Max(stack => GetOffsetTextMatchWeight(_stackTextCache[stack.Collectible.Code], searchText, searchWeightOffset + groupMemberOffset));

		return Math.Max(group, stacks);
	}

	private static float GetOffsetTextMatchWeight(TextCache cached, string searchText, float searchWeightOffset)
		=> GetOffsetTextMatchWeight(cached.Title, cached.All, searchText, searchWeightOffset);

	private static float GetOffsetTextMatchWeight(string title, string fullText, string searchText, float searchWeightOffset) {
		if (title.Equals(searchText, StringComparison.InvariantCultureIgnoreCase)) {
			return searchWeightOffset + 3f;
		}

		if (title.StartsWith(searchText + " ", StringComparison.InvariantCultureIgnoreCase)) {
			return searchWeightOffset + 2.75f + Math.Max(0, 15 - title.Length) / 100f;
		}

		if (title.StartsWith(searchText, StringComparison.InvariantCultureIgnoreCase)) {
			return searchWeightOffset + 2.5f + Math.Max(0, 15 - title.Length) / 100f;
		}

		if (title.CaseInsensitiveContains(searchText)) {
			return searchWeightOffset + 2f;
		}

		if (fullText.CaseInsensitiveContains(searchText)) {
			return searchWeightOffset + 1f;
		}

		return 0f;
	}
}
