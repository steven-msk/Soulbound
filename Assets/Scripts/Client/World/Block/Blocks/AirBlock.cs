using SoulboundBackend.Client.ItemSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Tilemaps;

namespace SoulboundBackend.Client.World.BlockSystem {
	public sealed class AirBlock : Block {
		public AirBlock() : base("air") {
		}

		public override string name { get; init; } = "Air";
		public override TileBase tileReference { get; init; } = null;
		public override BlockItem itemReference { get; init; } = null;

		public override IEnumerable<ItemStack> GetDrops(BlockState blockState, BreakSource source) {
			yield break;
		}

		protected override BlockState CreateDefaultState(BlockPropertyPool propertyPool) {
			return new(this, propertyPool.CreateEntries());
		}

		protected override void RegisterProperties(BlockPropertyPool pool) {
		}
	}
}
