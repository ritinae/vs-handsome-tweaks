using System;
using System.Collections.Generic;
using System.Linq;

using HarmonyLib;

using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

using Jakojaannos.HandsomeTweaks.Compatibility.ConfigLib;
using Jakojaannos.HandsomeTweaks.Compatibility.ACulinaryArtillery;
using Jakojaannos.HandsomeTweaks.Compatibility.XLib;
using Jakojaannos.HandsomeTweaks.Config;
using Jakojaannos.HandsomeTweaks.Modules;
using Jakojaannos.HandsomeTweaks.Modules.GroupedHandbookTab;
using Jakojaannos.HandsomeTweaks.Modules.MergeStacksOnGround.Patches;
using Jakojaannos.HandsomeTweaks.Modules.ResonatorMechanicalPower.GameContent;
using Jakojaannos.HandsomeTweaks.Modules.XLibLevelUpNotification.Client.Gui;

using static Jakojaannos.HandsomeTweaks.ModInfo;

using VSModSystem = Vintagestory.API.Common.ModSystem;

using MergeStacksOnGround = Jakojaannos.HandsomeTweaks.Modules.MergeStacksOnGround.ModuleInfo;
using StructuredLangFile = Jakojaannos.HandsomeTweaks.Modules.StructuredLangFile.ModuleInfo;
using KeepHandbookHistory = Jakojaannos.HandsomeTweaks.Modules.KeepHandbookHistory.ModuleInfo;
using ResonatorMechanicalPower = Jakojaannos.HandsomeTweaks.Modules.ResonatorMechanicalPower.ModuleInfo;


namespace Jakojaannos.HandsomeTweaks.ModSystem;

public class HandsomeTweaksModSystem : VSModSystem {
	private static HandsomeTweaksSettings Settings {
		get => HandsomeTweaksSettings.Instance;
	}

	private ConfigLibCompat? _configLib;
	private ACulinaryArtilleryCompat? _aCulinaryArtillery;
	private XLibCompat? _xlib;

	private List<ModModule> _modules = new();

	private Harmony? _harmony;

	internal event Action<HandsomeTweaksSettings>? SettingsLoaded;

	public override void Start(ICoreAPI api) {
		Mod.Logger.Debug("Handsome Tweaks Starting!");

		_modules.Add(new GroupedHandbookTab(Mod));

		HandsomeTweaksSettings.SyncWithModConfig(api);

		_configLib = ConfigLibCompat.TryInitialize(Mod.Logger, api);
		_configLib?.SubscribeToConfigChange();

		_aCulinaryArtillery = ACulinaryArtilleryCompat.TryInitialize(api);
		_xlib = XLibCompat.TryInitialize(api);

		api.RegisterBlockClass(nameof(BlockMPResonator), typeof(BlockMPResonator));

		_modules
			.Where(module => module.ShouldLoad(api))
			.Foreach(module => module.Start(api));

		ApplyPatches(api);
	}

	public override void StartClientSide(ICoreClientAPI api) {
		api
			.ChatCommands
			.Create("jj.debug.levelup")
			.WithDescription("Show the level up notification")
			.HandleWith((args) => new HudLevelUp(api, "Sneak", 100).OnDebugLevelUp());

		_modules
			.Where(module => module.ShouldLoad(api))
			.Where(module => module is IClientModModule)
			.Cast<IClientModModule>()
			.Foreach(module => module.StartClientSide(api));
	}

	private void ApplyPatches(ICoreAPI api) {
		Mod.Logger.Debug("Applying patches!");


		_harmony = new(MOD_ID);

		// FIXME: why isn't the new module thing not applying the patches?
		_harmony.PatchAll();
		/*
		if (Settings.Startup.IsStructuredTranslationEnabled) {
			_harmony.PatchCategory(StructuredLangFile.PATCH_CATEGORY);
		}

		if (Settings.Startup.IsMergeStacksOnGroundEnabled) {
			_harmony.PatchCategory(MergeStacksOnGround.PATCH_CATEGORY);

			if (Settings.MergeStacksOnGround.IsRenderPatchEnabled) {
				_harmony.PatchCategory(MergeStacksOnGround.PATCH_CATEGORY + EntityItemRendererPatch.RENDER_PATCH);
			}
		}

		if (Settings.Startup.IsKeepHandbookHistoryEnabled) {
			_harmony.PatchCategory(KeepHandbookHistory.PATCH_CATEGORY);
		}

		if (Settings.Startup.IsResonatorMechanicalPowerEnabled) {
			_harmony.PatchCategory(ResonatorMechanicalPower.PATCH_CATEGORY);
		}

		_xlib?.ApplyPatches(_harmony);
		_aCulinaryArtillery?.ApplyPatches(_harmony);

		_modules
			//.Where(module => module.ShouldLoad(api))
			.Foreach(module => module.ApplyPatches(_harmony));
			*/
	}

	public override void Dispose() {
		_xlib?.Dispose();
		_aCulinaryArtillery?.Dispose();

		_harmony?.UnpatchAll(MOD_ID);
	}
}
