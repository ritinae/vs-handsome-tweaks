using Jakojaannos.HandsomeTweaks.Config;

namespace Jakojaannos.HandsomeTweaks.Modules.CulinaryTweaks;

public static class CulinaryTweaks {
	public const string MODULE_ID = "culinarytweaks";
	public const string PATCH_CATEGORY = MODULE_ID;

	public static bool IsEnabled => HandsomeTweaksSettings.Instance.Startup.IsCulinaryTweaksEnabled;
}
