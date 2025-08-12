using System;

using Vintagestory.API.Common;

using Jakojaannos.HandsomeTweaks.Config;

namespace Jakojaannos.HandsomeTweaks.Modules;

public class HandsomeTweaksModSystem : ModSystem {
	internal event Action<HandsomeTweaksSettings>? SettingsLoaded;

	public override void StartPre(ICoreAPI api) {
		HandsomeTweaksSettings.SyncWithModConfig(Mod, api);
	}
}
