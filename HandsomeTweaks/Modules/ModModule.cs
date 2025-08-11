using System;
using System.Collections.Generic;

using HarmonyLib;

using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace Jakojaannos.HandsomeTweaks.Modules;

internal abstract class ModModule : IDisposable {
	protected abstract string ModuleId { get; }
	protected virtual IEnumerable<string> PatchCategories {
		get => new string[] { ModuleId };
	}

	protected Mod Mod { get; }
	private Harmony? _harmony;

	protected ModModule(Mod mod) {
		Mod = mod;
	}

	public virtual void ApplyPatches(Harmony harmony) {
		_harmony = harmony;
		PatchCategories.Foreach(_harmony.PatchCategory);
	}

	public virtual bool ShouldLoad(ICoreAPI api) {
		return ShouldLoad(api.Side);
	}

	public virtual bool ShouldLoad(EnumAppSide forSide) {
		return forSide switch {
			EnumAppSide.Client => this is IClientModModule,
			EnumAppSide.Server => this is IServerModModule,
			EnumAppSide.Universal => this is IClientModModule && this is IServerModModule,
		};
	}

	public virtual void Start(ICoreAPI api) {
	}

	public virtual void Dispose() {
		foreach (var category in PatchCategories) {
			_harmony?.UnpatchCategory(category);
		}
	}
}
