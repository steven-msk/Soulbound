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
		public LeafBlock() : base("leaves") { }
		public override string name { get; init; } = "Leaves";
		public override int minBreakLevel { get; init; } = 0;

		public override AssetKey GetRenderTileKey(BlockState blockState) => new("leaves");

		protected override BlockState GetDefaultState(BlockStateRegisterer registerer, BlockPropertyEntries propertyEntries) {
			return registerer.AddWithProperties(propertyEntries.With("persistent", true));
		}
	}
}
