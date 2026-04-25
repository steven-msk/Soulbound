using SoulboundEngine.Core.States;
using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.Client.World.BlockSystem.States {
	public class BlockState : Block.AbstractBlockState {
		public Block block { get; }

		public BlockState(Block owner, IDictionary<Property, object> entries) 
			: base(owner, new Entries(entries)) {
			this.block = owner;
		}

		protected override BlockState AsBlockState() => this;
	}
}
