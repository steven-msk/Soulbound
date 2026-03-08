using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Core.AssetManagement;
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

		public override IEnumerable<ItemStack> GetDrops(BlockState blockState, BreakSource source) {
			yield break;
		}

		public override AssetKey GetRenderTileKey(BlockState blockState) => null;
	}
}
