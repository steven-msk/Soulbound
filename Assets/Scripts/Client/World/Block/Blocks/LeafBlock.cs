using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Core.AssetManagement;
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
		public BlockProperty<bool> persistent;

		public LeafBlock() : base("leaves") { }
		public override string name { get; init; } = "Leaves";
		public override BreakRequirement breakRequirement { get; init; } = new BreakRequirement(0, ToolType.All, 10);

		//public override IEnumerable<ItemStack> GetDrops(BlockState blockState, BreakSource source) {
		//	if (source is not PlayerToolBreakSource) {
		//		yield break;
		//	}
		//	yield return new(Items.leavesBlock, 1);
		//}

		public override AssetKey GetRenderTileKey(BlockState blockState) => new("leaves");

		protected override BlockState GetDefaultState(BlockStateRegisterer registerer, BlockPropertyEntries propertyEntries) {
			return registerer.AddWithProperties(propertyEntries.With("persistent", true));
		}
	}
}
