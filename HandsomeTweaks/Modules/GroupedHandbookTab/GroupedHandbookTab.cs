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
using System.Text.RegularExpressions;

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
			var variantMappings = new Dictionary<AssetLocation, string[]> {
				// `game:ontree` -> `game:ontree-bellpepper`
				{ "game:ontree", new string[]{ "type" } },
				// `game:armor` -> `game:armor-{bodypart}`
				{ "game:armor", new string[]{ "bodypart" } },
			};

			// Custom groups. Items in these are removed from other groups
			// and added only to the custom group instead.
			var customGroups = new Dictionary<AssetLocation, Regex[]> {
				{
					"game:weapon-ruined", new Regex[] {
						new("^game:(axe|blade|club|knife|spear)-[a-zA-Z]+-ruined$")
					}
				},
				{
					"game:weapon-scrap", new Regex[] {
						new("^game:(axe|blade|club|knife|spear)-[a-zA-Z]+-scrap$")
					}
				},
				{
					"game:bags", new Regex[] {
						new("^game:.*backpack.*"),
						new("^game:(basket|linensack|miningbag).*"),
					}
				},
				{
					"game:bags-elk", new Regex[] {
						new("^game:.*saddlebags.*")
					}
				},
			};

			// For making things a bit more sensible for translations/configs
			var renames = new Dictionary<AssetLocation, AssetLocation> {
				// Vanilla crushed ores are simply `crushed-{ore}`
				{ "game:crushed", "game:crushed-ore" },
				// Not to be confused with chicken nuggies
				{ "game:nugget", "game:nugget-ore" },
			};

			var everything = api
				.World
				.Collectibles
				.Where(i => i?.Code != null)
				.SelectMany(i => i.GetHandBookStacks(api) ?? new List<ItemStack>(0));

			var groups = new ConcurrentDictionary<AssetLocation, List<ItemStack>>();
			foreach (var itemStack in everything) {
				var collectible = itemStack.Collectible;
				var isCustom = false;
				foreach (var (customGroup, patterns) in customGroups) {
					if (patterns.Any(p => p.IsMatch(collectible.Code))) {
						groups
							.GetOrAdd(customGroup, _ => new List<ItemStack>())
							.Add(itemStack);
						isCustom = true;
						break;
					}
				}

				if (isCustom) {
					continue;
				}

				var variant = collectible.VariantStrict;
				if (variant is null) {
					Mod.Logger.Error($"No variants for \"{collectible.Code}\"");
					continue;
				}

				// First pass: just reduce down to the bare "base" code
				var baseCode = collectible.Code;
				foreach (var (_, value) in variant) {
					baseCode = baseCode.WithoutPathPart(value);
				}

				// Second pass: apply exceptions
				var preservedKeys = variantMappings
					.GetValueOrDefault(baseCode) ?? Array.Empty<string>();
				var code = collectible.Code;
				foreach (var (key, value) in variant) {
					var isPreserved = preservedKeys.Contains(key);
					if (isPreserved) {
						continue;
					}

					code = code.WithoutPathPart(value);
				}

				// Post-processing: apply renames
				code = renames.Get(code, code);

				// Collect to groups
				groups
					.GetOrAdd(code, _ => new List<ItemStack>())
					.Add(itemStack);
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
