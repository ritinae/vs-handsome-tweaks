using System.Linq;

using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

using Jakojaannos.HandsomeTweaks.Config;
using Jakojaannos.HandsomeTweaks.Modules.GroupedHandbookTab.Client.Gui;
using System.Collections.Generic;
using Vintagestory.API.Util;
using System.Collections.Concurrent;
using System;
using System.Runtime.CompilerServices;
using Jakojaannos.HandsomeTweaks.Util;

namespace Jakojaannos.HandsomeTweaks.Modules.GroupedHandbookTab;

internal class GroupedHandbookTab : ModModule, IClientModModule {
	public const string MODULE_ID = "groupedhandbooktabs";
	public const string PATCH_CATEGORY = MODULE_ID;
	public static GroupedHandbookTab? Instance { get; private set; }
	public static bool IsEnabled => HandsomeTweaksSettings.Instance.Startup.IsGroupedHandbookTabEnabled;

	protected override string ModuleId => MODULE_ID;

	public GroupedHandbookTab(Mod mod) : base(mod) {
		Instance = this;
	}

	void IClientModModule.StartClientSide(ICoreClientAPI api) {
		var handbook = api.ModLoader.GetModSystem<ModSystemSurvivalHandbook>();
		if (handbook is not null) {
			RegisterCustomHandbookPages(api, handbook);
		}
	}
	private void RegisterCustomHandbookPages(
		ICoreClientAPI api,
		ModSystemSurvivalHandbook survivalHandbook
	) {
		survivalHandbook.OnInitCustomPages += (allHandbookPages) => {
			var stackPages = allHandbookPages
				.Where(page => page.CategoryCode == "stack" && page is GuiHandbookItemStackPage)
				.Cast<GuiHandbookItemStackPage>()
				.ToList();

			// Exceptions for the variant removal logic. Keeps the listed
			// variants as part of the base path.
			var variantAsPartOfBase = new Dictionary<AssetLocation, string[]> {
				// `game:ontree` -> `game:ontree-bellpepper`
				{ "game:ontree", new string[]{ "type" } },
				// `game:armor` -> `game:armor-{bodypart}`
				{ "game:armor", new string[]{ "bodypart" } }
			};

			// For making things a bit more sensible for translations/configs
			var remappings = new Dictionary<AssetLocation, AssetLocation> {
				// Vanilla crushed ores are simply `crushed-{ore}`
				{ "game:crushed", "game:crushed-ore" },
				// Not to be confused with chicken nuggies
				{ "game:nugget", "game:nugget-ore" },
			};

			var allItems = api
				.World
				.Items
				.Where(i => i?.Code != null)
				.SelectMany(i => i.GetHandBookStacks(api) ?? new List<ItemStack>(0));

			var groups = new ConcurrentDictionary<AssetLocation, List<ItemStack>>();
			foreach (var itemStack in allItems) {
				var item = itemStack.Item;
				var variant = item.VariantStrict;
				if (variant is null) {
					Mod.Logger.Error($"No variants for \"{item.Code}\"");
					continue;
				}

				// First pass: just reduce down to the bare "base" code
				var baseCode = item.Code;
				foreach (var (_, value) in variant) {
					baseCode = baseCode.WithoutPathPart(value);
				}

				// Second pass: apply exceptions
				var preservedKeys = variantAsPartOfBase
					.GetValueOrDefault(baseCode) ?? Array.Empty<string>();
				var code = item.Code;
				foreach (var (key, value) in variant) {
					var isPreserved = preservedKeys.Contains(key);
					if (isPreserved) {
						continue;
					}

					code = code.WithoutPathPart(value);
				}

				// Post-processing: apply remappings
				code = remappings.Get(code, code);

				// Collect to groups
				var group = groups.GetOrAdd(code, _ => new List<ItemStack>());
				group.Add(itemStack);
			}

			foreach (var (key, group) in groups) {
				// Special case: only one item in group
				if (group.Count == 1) {
					var itemStack = group[0];
					allHandbookPages.Add(new GuiHandbookGroupedSingleItemPageWrapper(api, itemStack));
					continue;
				}

				var groupPage = new GuiHandbookGroupedItemPage(
					api,
					group,
					key
				);
				allHandbookPages.Add(groupPage);
			}
		};
	}
}
