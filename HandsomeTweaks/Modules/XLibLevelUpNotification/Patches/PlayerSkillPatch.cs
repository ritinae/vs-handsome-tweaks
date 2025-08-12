using HarmonyLib;

using Vintagestory.API.Client;

using XLib.XLeveling;

using Jakojaannos.HandsomeTweaks.Modules.XLibLevelUpNotification.Client.Gui;

namespace Jakojaannos.HandsomeTweaks.Modules.XLibLevelUpNotification.Patches;

[HarmonyPatchCategory(XLibLevelUpNotification.PATCH_CATEGORY)]
public static class PlayerSkillPatch {
	public readonly struct SetExperiencePatchState {
		public required int LevelBefore { get; init; }
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(PlayerSkill), nameof(PlayerSkill.Experience), MethodType.Setter)]
	public static void SetExperiencePrefix(PlayerSkill __instance, ref SetExperiencePatchState __state) {
		__state = new() {
			LevelBefore = __instance.Level,
		};
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(PlayerSkill), nameof(PlayerSkill.Experience), MethodType.Setter)]
	public static void SetExperiencePostfix(PlayerSkill __instance, SetExperiencePatchState __state) {
		if (__instance.Level <= __state.LevelBefore) {
			return;
		}

		var api = __instance.Skill.XLeveling.Api;
		if (api is ICoreClientAPI capi) {
			// FIXME: queue if already open
			capi.Event.EnqueueMainThreadTask(() =>
				new HudLevelUp(capi, __instance.Skill, __instance.Level).TryOpen(), "OnLevelUpOpenNotification"
			);
		}
	}
}
