using SoulboundEngine.Core.States;
using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.Client.World.Block.State {
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
			return $"block[{this.block}, properties={this.GetEntries()}]";
		}
	}
}
