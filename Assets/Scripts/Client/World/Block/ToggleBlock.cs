using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Core.AssetManagement;
using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.BlockSystem {
	public sealed class ToggleBlock : Block, IBlockInteractionHandler {
		public BlockState on { get; private set; }
		public BlockState off { get; private set; }
		public override string name { get; init; } = "Toggle Block";
		public override BlockItem itemReference { get; init; } = null;
		public override AssetKey tileKey { get; init; } = new("WhiteSquareTile");

		public ToggleBlock() : base("toggleBlock") { }


		public void OnInteract(Level level, BlockPos blockPos, BlockState blockState) {
			bool isOn = blockState.Get<bool>("on");
			level.SetBlockState(blockPos, isOn ? off : on);
			Logger.LogInfo("block at {} is now {}", blockPos, isOn ? "on" : "off");
		}

		protected override BlockState CreateDefaultState(BlockStateRegisterer registerer, BlockPropertyEntries propertyEntries) {
			return registerer.AddWithProperties(propertyEntries.With("on", true));
		}

		protected override void CreateStates(BlockStateRegisterer registerer, BlockPropertyEntries properties) {
			on = defaultState;
			off = registerer.AddWithProperties(properties.With("on", false));
		}

		public override IEnumerable<ItemStack> GetDrops(BlockState blockState, BreakSource source) {
			yield break;
		}
	}
}
