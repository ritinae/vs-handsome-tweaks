using Jakojaannos.HandsomeTweaks.Config;

using Vintagestory.API.Common;

namespace Jakojaannos.HandsomeTweaks.Modules.FirepitTweaks;

internal class FirepitTweaks : ModModule, IClientModModule, IServerModModule {
	public const string MODULE_ID = "firepittweaks";
	public const string PATCH_CATEGORY = MODULE_ID;

	public static bool IsEnabled => HandsomeTweaksSettings.Instance.Startup.IsFirepitTweaksEnabled;

	protected override string ModuleId => MODULE_ID;

	public FirepitTweaks(Mod mod) : base(mod) {
	}
}
