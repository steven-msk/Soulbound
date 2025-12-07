using SoulboundBackend.Client.ItemSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Tilemaps;

namespace SoulboundBackend.Client.World.BlockSystem {
	public sealed class AirBlock : Block {
		public override string name => "Air";
		public override TileBase tileReference => null;
		public override BlockItem itemReference => null;

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
