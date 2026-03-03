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
		//public override TileBase tileReference { get; init; } = ResourceManager.Get<TileBase, ResourceGroups.Tiles>("leaves");
		public override BlockItem itemReference { get; init; } = Items.leavesBlock;
		public override BreakRequirement breakRequirement { get; init; } = new BreakRequirement(0, ToolType.All, 10);
		public override AssetKey tileKey { get; init; } = new("leaves");
		// protected

		public override IEnumerable<ItemStack> GetDrops(BlockState blockState, BreakSource source) {
			if (source is not PlayerToolBreakSource) {
				yield break;
			}
			yield return new(Items.leavesBlock, 1);
		}

		protected override BlockState CreateDefaultState(BlockStateRegisterer registerer, BlockPropertyEntries propertyEntries) {
			return registerer.AddWithProperties(propertyEntries.With(persistent, true));
		}

		protected override void RegisterProperties(BlockPropertyPool pool) {
			persistent = pool.Register<bool>("persistent");
		}
	}
}
