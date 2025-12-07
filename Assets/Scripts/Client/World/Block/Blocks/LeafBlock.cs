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
		public BlockProperty<bool> persistent;

		public LeafBlock() : base("leaves") {
		}

		public override string name { get; } = "Leaves";

		public override TileBase tileReference => ResourceManager.Get<TileBase, ResourceGroups.Tiles>("leaves");

		public override BlockItem itemReference => Items.leavesBlock;

		public override BreakRequirement breakRequirement => new BreakRequirement(0, ToolType.All, 10);

		public override IEnumerable<ItemStack> GetDrops(BlockState blockState, BreakSource source) {
			if (blockState.Get(persistent)) {
				yield break;
			}
			yield return new(itemReference, 1);
		}

		protected override BlockState CreateDefaultState(BlockPropertyPool propertyPool) {
			return new(this, propertyPool.CreateEntries().With(persistent, true));
		}

		protected override void RegisterProperties(BlockPropertyPool pool) {
			persistent = pool.Register<bool>("persistent");
		}
	}
}
