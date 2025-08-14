using Vintagestory.API.Common;

using Jakojaannos.HandsomeTweaks.Config;

namespace Jakojaannos.HandsomeTweaks.Modules;

public class HandsomeTweaksModSystem : ModSystem {
	public override void StartPre(ICoreAPI api) {
		HandsomeTweaksSettings.SyncWithModConfig(api);
	}
}
