using SoulboundEngine.Client.World.BlockSystem.Render;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Common;
using SoulboundEngine.Common.Math;
using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.States;

namespace SoulboundEngine.Client.World.BlockSystem {
	[PROTOTYPICAL]
	public sealed class NeighborReactiveBlock : Block, INeighborUpdateHandler {
		public static readonly Property<bool> on = BoolProperty.Of("on");
		private readonly BlockState active;
		private readonly BlockState inactive;

		public NeighborReactiveBlock(Settings settings) 
			: base(settings) {
			this.SetDefaultState(this.DefaultState.With(on, false));
			this.inactive = this.DefaultState;
			this.active = this.DefaultState.With(on, true);
		}

		protected override void AppendProperties(StateManager<Block, BlockState>.Builder builder) {
			builder.Add(on);
		}

		public void OnNeighborChanged(Level level, BlockPos selfPos, BlockPos neighborPos) {
			bool shouldActivate = false;

			foreach (var pos in selfPos.GetCardinalNeighbors()) {
				BlockState blockState = level.GetBlockState(pos);
				// provisory for the toggle block
				if (blockState?.block is ToggleBlock && blockState.TryGet(on, out bool isOn) && isOn) {
					shouldActivate = true;
					break;
				}
			}

			BlockState selfState = level.GetBlockState(selfPos);
			bool isActive = selfState.Get(on);

			if (shouldActivate != isActive) {
				level.SetBlockState(selfPos, shouldActivate ? this.active : this.inactive);
			}
		}

		public override BlockRenderData GetRenderData(BlockState blockState) {
			return new BlockRenderData(blockState.Get(on)
				? new AssetKey("ReactActiveTile")
				: new AssetKey("ReactInactiveTile")
			);
		}
	}
}
