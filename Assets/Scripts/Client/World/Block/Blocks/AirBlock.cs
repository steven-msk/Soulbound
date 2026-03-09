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
		public override string name { get; init; } = "Air";
		public override int minBreakLevel { get; init; } = 0;

		public AirBlock() : base("air") {
		}

		public override AssetKey GetRenderTileKey(BlockState blockState) => null;
	}
}
