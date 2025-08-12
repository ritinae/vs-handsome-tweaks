using System;
using System.Linq;

using Vintagestory.API.Common;

using static Jakojaannos.HandsomeTweaks.ModInfo;

namespace Jakojaannos.HandsomeTweaks.Config;

internal sealed class HandsomeTweaksSettings {
	public const string FILENAME = $"{AUTHOR_DOMAIN}/{MOD_ID}.json";

	public readonly StartupSettings Startup = new();
	public readonly MergeStacksOnGroundSettings MergeStacksOnGround = new();
	public readonly KeepHandbookHistorySettings KeepHandbookHistory = new();

	// Must contain a property `Is<ModuleName>Enabled` for every mod module.
	internal sealed class StartupSettings {
		public bool IsMergeStacksOnGroundEnabled { get; set; } = true;
		public bool IsKeepHandbookHistoryEnabled { get; set; } = true;
		public bool IsXLibLevelUpNotificationEnabled { get; set; } = true;
		public bool IsResonatorMechanicalPowerEnabled { get; set; } = true;
		public bool IsCulinaryTweaksEnabled { get; set; } = true;
		public bool IsPathfinderClassEnabled { get; set; } = true;
		public bool IsGroupedHandbookTabEnabled { get; set; } = true;
		public bool IsFirepitTweaksEnabled { get; set; } = true;
	}

	internal sealed class MergeStacksOnGroundSettings {
		public bool IsRenderPatchEnabled { get; set; } = true;
		public int MaxRenderedStacks { get; set; } = 10;
		public MergeStrategy Strategy { get; set; } = MergeStrategy.OnCollidedDelayed;

		public enum MergeStrategy {
			Continuous,
			OnCollidedDelayed
		}
	}

	internal sealed class KeepHandbookHistorySettings {
		public bool IsHistoryPersistent { get; set; } = true;
		public int HistoryMaxSize { get; set; } = 50;
	}

	private static ILogger? s_logger;
	internal static HandsomeTweaksSettings Instance { get; set; } = new();

	internal static void SyncWithModConfig(Mod mod, ICoreAPI api) {
		s_logger = mod.Logger;
		try {
			var existingConfig = api.LoadModConfig<HandsomeTweaksSettings>(FILENAME);
			if (existingConfig is not null) {
				Instance = existingConfig;
			}
		} finally {
			// Always store mod config in case new fields have been added.
			api.StoreModConfig(Instance, FILENAME);
		}
	}

	internal static bool IsModuleEnabled<T>() {
		return IsModuleEnabled(typeof(T));
	}

	internal static bool IsModuleEnabled(Type type) {
		var props = typeof(StartupSettings).GetProperties();
		var moduleName = type.Name;

		var settingName = $"Is{moduleName}Enabled";
		var prop = props.FirstOrDefault(p => p.Name == settingName);
		if (prop is null) {
			s_logger?.Error($"\"{settingName}\" is missing!");
			return false;
		}

		return prop != null && prop.GetValue(Instance.Startup) is bool value && value;
	}
}
