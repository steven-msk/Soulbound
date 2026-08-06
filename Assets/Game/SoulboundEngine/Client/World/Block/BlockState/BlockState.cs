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

		public override string ToString() {
			return $"block[{this.block}, properties={this.GetEntries()}]";
		}
	}
}
