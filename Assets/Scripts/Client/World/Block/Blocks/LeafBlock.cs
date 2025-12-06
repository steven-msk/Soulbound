using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Tilemaps;
using static Unity.Collections.AllocatorManager;

namespace SoulboundBackend.Client.World.BlockSystem {
	public class LeafBlock : Block {
		public LeafBlock() : base(StateCaching.Predefined()) {
		}

		public override string name => "Leaves";

		public override TileBase tileReference => ResourceManager.Get<TileBase, ResourceGroups.Tiles>("leaves");

		public override BlockItem itemReference => Items.leavesBlock;

		public override BreakRequirement breakRequirement => new BreakRequirement(0, ToolType.All, 10);

		//protected override BlockState CreateDefaultState() {
		//	Func<BlockState, BreakSource, bool> dropPredicate = (blockState, breakSource) => {
		//		return breakSource is PlayerToolBreakSource || blockState.Get<bool>("persistent");
		//	};
		//	return new BlockState(this, null);
		//}

		//protected override void RegisterProperties() {
		//	RegisterProperty(new BlockProperty<bool>("persistent"), false);
		//}

		//public override BlockState Place(ItemStack itemStack, BlockPos blockPos) {
		//	return defaultState.With_obsolete("persistent", true);
		//}

		public override IEnumerable<ItemStack> GetDrops(BlockState blockState, BreakSource source) {
			throw new NotImplementedException();
		}

		protected override BlockState CreateDefaultState(BlockPropertyPool propertyPool) {
			throw new NotImplementedException();
		}

		protected override void RegisterProperties(BlockPropertyPool pool) {
			throw new NotImplementedException();
		}
	}
}
