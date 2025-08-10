using System.Collections.Generic;

using Jakojaannos.HandsomeTweaks.Modules.CulinaryTweaks;

using Vintagestory.API.Common;

namespace Jakojaannos.HandsomeTweaks.Compatibility.ACulinaryArtillery;

internal class ACulinaryArtilleryCompat : ModIntegration {
	public const string MOD_ID = "aculinaryartillery";

	protected override IEnumerable<string> PatchCategories => new string[] {
		CulinaryTweaks.PATCH_CATEGORY,
	};

	internal static ACulinaryArtilleryCompat? TryInitialize(ICoreAPI api) {
		if (!api.ModLoader.IsModEnabled(MOD_ID)) {
			return null;
		}

		return new();
	}
}