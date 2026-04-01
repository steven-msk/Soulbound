using SoulboundEngine.Client.World.BlockSystem.Render;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Common;
using SoulboundEngine.Common.Math;
using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Client.World.BlockSystem {
	[PROTOTYPICAL]
	public sealed class NeighborReactiveBlock : Block, INeighborUpdateHandler {
		private static readonly Identifier identifier = new("neighborReactiveBlock");
		public BlockState active { get; private set; }
		public BlockState inactive { get; private set; }
		public override string name { get; init; } = "Neighbor Reactive Block";
		public override int minBreakLevel { get; init; } = 0;

		public NeighborReactiveBlock() : base(identifier) {
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

		public override BlockRenderData GetRenderData(BlockState blockState) {
			return new BlockRenderData(blockState.Get<bool>("on")
				? new AssetKey("ReactActiveTile")
				: new AssetKey("ReactInactiveTile")
			);
		}
	}
}
