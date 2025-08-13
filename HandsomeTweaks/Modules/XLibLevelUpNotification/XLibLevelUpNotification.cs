using Vintagestory.API.Client;
using Vintagestory.API.Common;

using Jakojaannos.HandsomeTweaks.Modules.XLibLevelUpNotification.Client.Gui;
using Jakojaannos.HandsomeTweaks.Compatibility;

namespace Jakojaannos.HandsomeTweaks.Modules.XLibLevelUpNotification;

internal class XLibLevelUpNotification : ModModule<HandsomeTweaksModSystem> {
	public const string MODULE_ID = "xliblevelupnotification";
	public const string PATCH_CATEGORY = MODULE_ID;

	protected override string ModuleId => MODULE_ID;

	public override bool ShouldLoad(ICoreAPI api) {
		return base.ShouldLoad(api) && api.ModLoader.IsModEnabled(ModIds.XLIB) && !api.ModLoader.IsModEnabled(ModIds.XSKILLS_GILDED);
	}

	public override bool ShouldLoad(EnumAppSide forSide) {
		return forSide == EnumAppSide.Client;
	}

	public override void StartClientSide(ICoreClientAPI api) {
		api
			.ChatCommands
			.Create("jj.debug.levelup")
			.WithDescription("Show the level up notification")
			.HandleWith((args) => new HudLevelUp(api, "Sneak", 100).OnDebugLevelUp());
	}
}
