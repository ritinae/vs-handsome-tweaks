using Jakojaannos.HandsomeTweaks.Config;

namespace Jakojaannos.HandsomeTweaks.Modules.XLibLevelUpNotification;

public static class XLibLevelUpNotification {
	public const string MODULE_ID = "xliblevelupnotification";
	public const string PATCH_CATEGORY = MODULE_ID;

	public static bool IsEnabled => HandsomeTweaksSettings.Instance.Startup.IsXLibLevelUpNotificationEnabled;
}
