#nullable enable

namespace SoulboundEngine.World.Block.State {
	using SoulboundEngine.States;
	using System.Collections.Generic;

	public class BlockState : AbstractBlock.AbstractBlockState {
		public Block block { get; }

		public BlockState(Block owner, IDictionary<Property, object> entries) 
			: base(owner, new Entries(entries)) {
			this.block = owner;
		}

		protected override BlockState AsBlockState() => this;

		public bool IsAir() => this.block == Blocks.AIR;

		public bool IsOf(Block block) => this.block == block;

		public override string ToString() {
			return $"{this.block}{(this.GetEntries().Count == 0 ? "" : this.GetEntries().ToString())}";
		}
	}
}
