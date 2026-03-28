using SoulboundEngine.Client.Players;
using SoulboundEngine.Client.World.BlockSystem.Render;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Client.World.BlockSystem.TileEntities;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Common;
using SoulboundEngine.Core.Assets;
using UnityEngine;

namespace SoulboundEngine.Client.World.BlockSystem {
	[PROTOTYPICAL]
	public sealed class AreaTriggerBlock : Block {
		private static BlockState inArea;
		private static BlockState notInArea;

		public override string name { get; init; } = "Area Trigger Block";
		public override int minBreakLevel { get; init; } = 0;

		public AreaTriggerBlock() : base("areaTriggerBlock") {
		}

		public override bool HasTileEntity(Level level, BlockPos blockPos, BlockState blockState) {
			return true;
		}

		public override TileEntity GetTileEntity(Level level, BlockPos blockPos) {
			ObjectTileEntity tileEntity = new(level, blockPos);

			tileEntity.onTriggerEnter += player => OnAreaEnter(level, blockPos, player);
			tileEntity.onTriggerExit += player => OnAreaExit(level, blockPos, player);

			return tileEntity;
		}

		private void OnAreaEnter(Level level, BlockPos selfPos, Player player) {
			level.SetBlockState(selfPos, inArea);
		}

		private void OnAreaExit(Level level, BlockPos selfPos, Player player) {
			level.SetBlockState(selfPos, notInArea);
		}

		protected override void CreateStates(IBlockStateRegisterer registerer, BlockPropertyEntries properties) {
			inArea = registerer.AddWithProperties(properties.With("inArea", true));
			notInArea = registerer.AddWithProperties(properties.With("inArea", false));
		}

		protected override BlockState GetDefaultState(IBlockStateRegisterer registerer, BlockPropertyEntries properties) {
			return notInArea;
		}

		public override BlockRenderData GetRenderData(BlockState blockState) {
			return new BlockRenderData {
				tileKey = new AssetKey("AreaTriggerTile"),
				color = blockState.Get<bool>("inArea")
					? Color.red
					: Color.green
			};
		}
	}
}
