using SoulboundBackend.Client.ItemSystem;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

#nullable enable

namespace SoulboundBackend.Client.World.BlockSystem {
	public class GenericBlock : Block {
		public override string name { get; }
		public override TileBase tileReference { get; }
		public override BlockItem itemReference { get; }
		public override BreakRequirement? breakRequirement { get; }

		public GenericBlock(
				string name,
				TileBase tileReference,
				BlockItem itemReference,
				BreakRequirement? breakRequirement
			) : base(StateCaching.Static()) {
			this.name = name;
			this.tileReference = tileReference;
			this.itemReference = itemReference;
			this.breakRequirement = breakRequirement;
		}

		public override IEnumerable<ItemStack> GetDrops(BlockState blockState, BreakSource source) {
			yield return new(itemReference, 1);
		}

		protected override void RegisterProperties(BlockPropertyPool pool) {
		}

		protected override BlockState CreateDefaultState(BlockPropertyPool propertyPool) {
			return new BlockState(this, propertyPool.CreateEntries());
		}
	}
}
