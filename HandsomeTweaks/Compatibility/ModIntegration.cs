using System;
using System.Collections.Generic;

using HarmonyLib;

using Vintagestory.API.Util;

namespace Jakojaannos.HandsomeTweaks.Compatibility;

internal abstract class ModIntegration : IDisposable {
	protected abstract IEnumerable<string> PatchCategories { get; }

	private Harmony? _harmony;

	internal virtual void ApplyPatches(Harmony harmony) {
		_harmony = harmony;
		PatchCategories.Foreach(_harmony.PatchCategory);
	}

	public virtual void Dispose() {
		foreach (var category in PatchCategories) {
			_harmony?.UnpatchCategory(category);
		}
	}
}
