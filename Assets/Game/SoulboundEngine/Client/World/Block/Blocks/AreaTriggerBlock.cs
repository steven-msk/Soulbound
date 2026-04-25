using SoulboundEngine.Client.Players;
using SoulboundEngine.Client.World.BlockSystem.Render;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Client.World.BlockSystem.TileEntities;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Common;
using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.States;
using UnityEngine;

namespace SoulboundEngine.Client.World.BlockSystem {
	[PROTOTYPICAL]
	public sealed class AreaTriggerBlock : Block {
		public static readonly Property<bool> inArea = BoolProperty.Of("inArea");

		public AreaTriggerBlock(Settings settings) 
			: base(settings) {
			this.SetDefaultState(this.DefaultState.With(inArea, false));
		}

		protected override void AppendProperties(StateManager<Block, BlockState>.Builder builder) {
			builder.Add(inArea);
		}

		public override bool HasTileEntity(Level level, BlockPos blockPos, BlockState blockState) {
			return true;
		}

		public override TileEntity GetTileEntity(Level level, BlockPos blockPos) {
			ObjectTileEntity tileEntity = new(level, blockPos);

			tileEntity.onTriggerEnter += player => this.OnAreaEnter(level, blockPos, player);
			tileEntity.onTriggerExit += player => this.OnAreaExit(level, blockPos, player);

			return tileEntity;
		}

		private void OnAreaEnter(Level level, BlockPos selfPos, Player player) {
			level.SetBlockState(selfPos, this.DefaultState.With(inArea, true));
		}

		private void OnAreaExit(Level level, BlockPos selfPos, Player player) {
			level.SetBlockState(selfPos, this.DefaultState.With(inArea, false));
		}

		public override BlockRenderData GetRenderData(BlockState blockState) {
			return new BlockRenderData(
				new AssetKey("AreaTriggerTile"),
				blockState.Get(inArea)
					? Color.red
					: Color.green
			);
		}
	}
}
