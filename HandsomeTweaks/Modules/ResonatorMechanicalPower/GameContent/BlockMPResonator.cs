using System;
using System.Collections.Generic;

using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;
using Vintagestory.GameContent.Mechanics;

namespace Jakojaannos.HandsomeTweaks.Modules.ResonatorMechanicalPower.GameContent;

public class BlockMPResonator : BlockMPBase {
	/* Copied from vanilla BlockResonator */
	private WorldInteraction[] _interactions = Array.Empty<WorldInteraction>();

	public override void OnLoaded(ICoreAPI api) {
		base.OnLoaded(api);

		if (api.Side != EnumAppSide.Client || api is not ICoreClientAPI capi) {
			return;
		}
		_interactions = ObjectCacheUtil.GetOrCreate(api, "echoChamberBlockInteractions", () => {
			var echochamberStacks = new List<ItemStack>();

			foreach (var obj in api.World.Collectibles) {
				if (obj.Attributes?.IsTrue("isPlayableDisc") == true) {
					echochamberStacks.Add(new ItemStack(obj));
				}
			}

			return new WorldInteraction[] {
				new() {
					ActionLangCode = "blockhelp-bloomery-playdisc",
					HotKeyCode = null,
					MouseButton = EnumMouseButton.Right,
					Itemstacks = echochamberStacks.ToArray(),
					GetMatchingStacks = (wi, bs, es) => {
						if (api.World.BlockAccessor.GetBlockEntity(bs.Position) is not BlockEntityResonator bee || !bee.HasDisc) {
							return wi.Itemstacks;
						}

						return null;
					}
				},
				new() {
					ActionLangCode = "blockhelp-bloomery-takedisc",
					HotKeyCode = null,
					MouseButton = EnumMouseButton.Right,
					ShouldApply = (wi, bs, es) => {
						return api.World.BlockAccessor.GetBlockEntity(bs.Position) is BlockEntityResonator bee && bee.HasDisc;
					}
				}
			};
		});
	}

	public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel) {
		if (blockSel.Position == null) {
			return base.OnBlockInteractStart(world, byPlayer, blockSel);
		}

		if (world.BlockAccessor.GetBlockEntity(blockSel.Position) is BlockEntityResonator beec) {
			beec.OnInteract(world, byPlayer);
		}

		return true;
	}

	public override WorldInteraction[] GetPlacedBlockInteractionHelp(IWorldAccessor world, BlockSelection selection, IPlayer forPlayer) {
		return _interactions.Append(base.GetPlacedBlockInteractionHelp(world, selection, forPlayer));
	}

	/* BlockMPBase impl + mechanical power connection (copied/adapted from BlockQuern) */

	public override bool TryPlaceBlock(IWorldAccessor world, IPlayer byPlayer, ItemStack itemstack, BlockSelection blockSel, ref string failureCode) {
		var ok = base.TryPlaceBlock(world, byPlayer, itemstack, blockSel, ref failureCode);
		if (ok) {
			tryConnect(world, byPlayer, blockSel.Position, BlockFacing.DOWN);
		}

		return ok;
	}

	public override void DidConnectAt(IWorldAccessor world, BlockPos pos, BlockFacing face) {
		// NOOP
	}

	public override bool HasMechPowerConnectorAt(IWorldAccessor world, BlockPos pos, BlockFacing face) {
		return face == BlockFacing.DOWN;
	}
}
