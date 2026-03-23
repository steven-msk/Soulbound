using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.World.BlockSystem.States;
using SoulboundBackend.Client.World.LevelDomain;
using SoulboundBackend.Common;
using SoulboundBackend.Common.Math;
using SoulboundBackend.Core.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.BlockSystem {
	[PROTOTYPICAL]
	public sealed class NeighborReactiveBlock : Block, INeighborUpdateHandler {
		public BlockState active { get; private set; }
		public BlockState inactive { get; private set; }
		public override string name { get; init; } = "Neighbor Reactive Block";
		public override int minBreakLevel { get; init; } = 0;

		public NeighborReactiveBlock() : base("neighborReactiveBlock") {
		}

		public override AssetKey GetRenderTileKey(BlockState blockState) {
			return blockState.Get<bool>("on")
				? new AssetKey("ReactActiveTile")
				: new AssetKey("ReactInactiveTile");
		}

		protected override BlockState GetDefaultState(IBlockStateRegisterer registerer, BlockPropertyEntries properties) {
			return inactive;
		}

		protected override void CreateStates(IBlockStateRegisterer registerer, BlockPropertyEntries properties) {
			inactive = registerer.AddWithProperties(properties.With("on", false));
			active = registerer.AddWithProperties(properties.With("on", true));
		}

		public void OnNeighborChanged(Level level, BlockPos selfPos, BlockPos neighborPos) {
			bool shouldActivate = false;

			foreach (var pos in selfPos.GetCardinalNeighbors()) {
				BlockState blockState = level.GetBlockState(pos);
				// provisory for the toggle block
				if (blockState?.block is ToggleBlock && blockState.TryGet("on", out bool isOn) && isOn) {
					shouldActivate = true;
					break;
				}
			}

			BlockState selfState = level.GetBlockState(selfPos);
			bool isActive = selfState.Get<bool>("on");

			if (shouldActivate != isActive) {
				level.SetBlockState(selfPos, shouldActivate ? active : inactive);
			}
		}
	}
}
