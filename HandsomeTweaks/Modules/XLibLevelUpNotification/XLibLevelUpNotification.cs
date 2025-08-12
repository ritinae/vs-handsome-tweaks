using Vintagestory.API.Client;

using Jakojaannos.HandsomeTweaks.Modules.XLibLevelUpNotification.Client.Gui;
using Vintagestory.API.Common;

namespace Jakojaannos.HandsomeTweaks.Modules.XLibLevelUpNotification;

internal class XLibLevelUpNotification : ModModule<HandsomeTweaksModSystem> {
	public const string MODULE_ID = "xliblevelupnotification";
	public const string PATCH_CATEGORY = MODULE_ID;

	protected override string ModuleId => MODULE_ID;

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
