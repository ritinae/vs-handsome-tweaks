using HarmonyLib;

using Vintagestory.API.Common;

namespace Jakojaannos.HandsomeTweaks.Modules.StructuredLangFile;

public class StructuredLangFile : ModSystem {
	public const string MODULE_ID = "structuredlangfile";
	public const string PATCH_CATEGORY = MODULE_ID;

	private Harmony? _harmony;

	// Execute VERY early
	public override double ExecuteOrder() {
		return 0.05;
	}

	public override void StartPre(ICoreAPI api) {
		_harmony = new($"{Mod.Info.ModID}-{MODULE_ID}");
		_harmony.PatchCategory(PATCH_CATEGORY);
	}

	public override void Dispose() {
		_harmony?.UnpatchCategory(PATCH_CATEGORY);
	}
}
