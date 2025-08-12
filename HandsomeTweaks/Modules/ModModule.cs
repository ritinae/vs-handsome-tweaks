using System.Collections.Generic;

using HarmonyLib;

using Jakojaannos.HandsomeTweaks.Config;

using Vintagestory.API.Common;

namespace Jakojaannos.HandsomeTweaks.Modules;

internal abstract class ModModule<TParent> : ModSystem where TParent : ModSystem {
	protected abstract string ModuleId { get; }
	protected virtual IEnumerable<string> PatchCategories {
		get => new string[] { ModuleId };
	}

	internal virtual bool IsEnabled => HandsomeTweaksSettings.IsModuleEnabled(GetType());

	private Harmony? _harmony;
	private ModSystem? _parent;

	public override bool ShouldLoad(ICoreAPI api) {
		return base.ShouldLoad(api) && IsEnabled;
	}

	public override double ExecuteOrder() {
		return (_parent?.ExecuteOrder() ?? base.ExecuteOrder()) + 0.01;
	}

	public override void StartPre(ICoreAPI api) {
		_parent = api.ModLoader.GetModSystem<TParent>();
	}

	public override void Start(ICoreAPI api) {
		_harmony ??= CreatePatcher();
		ApplyPatches(_harmony);
	}

	protected virtual void ApplyPatches(Harmony harmony) {
		_harmony = harmony;
		foreach (var module in PatchCategories) {
			_harmony.PatchCategory(module);
		}
	}

	public override void Dispose() {
		foreach (var category in PatchCategories) {
			_harmony?.UnpatchCategory(category);
		}
	}

	protected virtual Harmony CreatePatcher() {
		return new Harmony($"{Mod.Info.ModID}-{ModuleId}");
	}
}
