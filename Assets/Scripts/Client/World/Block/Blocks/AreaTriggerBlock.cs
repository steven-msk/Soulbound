using SoulboundBackend.Common;
using SoulboundBackend.Core.AssetManagement;
using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.BlockSystem {
	[PROTOTYPICAL]
	public sealed class AreaTriggerBlock : Block {
		public override string name { get; init; } = "Area Trigger Block";
		public override int minBreakLevel { get; init; } = 0;

		public override AssetKey GetRenderTileKey(BlockState blockState) {
			return new AssetKey("AreaTriggerTile");
		}

		public AreaTriggerBlock() : base("areaTriggerBlock") {
		}

		public override bool HasTileEntity(Level level, BlockPos blockPos, BlockState blockState) {
			return true;
		}

		public override TileEntity GetTileEntity(Level level, BlockPos blockPos) {
			ObjectTileEntity tileEntity = new(level, blockPos);

			tileEntity.onTriggerEnter += OnAreaEnter;
			tileEntity.onTriggerExit += OnAreaExit;
			tileEntity.onDestroyed += () => {
				tileEntity.onTriggerEnter -= OnAreaEnter;
				tileEntity.onTriggerExit -= OnAreaExit;
			};

			return tileEntity;
		}

		private void OnAreaEnter(Player player) {
			Logger.LogInfo("player entered area");
		}

		private void OnAreaExit(Player player) {
			Logger.LogInfo("player left area");
		}
	}
}
