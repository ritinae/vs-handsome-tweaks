using System.Collections.Generic;

using HarmonyLib;

using Jakojaannos.HandsomeTweaks.Modules.XLibLevelUpNotification;

using Vintagestory.API.Common;

namespace Jakojaannos.HandsomeTweaks.Compatibility.XLib;

internal class XLibCompat : ModIntegration {
	public const string MOD_ID = "xlib";

	protected override IEnumerable<string> PatchCategories => new string[] {
		XLibLevelUpNotification.PATCH_CATEGORY,
	};

	internal static XLibCompat? TryInitialize(ICoreAPI api) {
		if (!api.ModLoader.IsModEnabled(MOD_ID)) {
			return null;
		}

		return new();
	}
}