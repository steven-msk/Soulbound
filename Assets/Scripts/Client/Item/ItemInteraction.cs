using SoulboundBackend.Client.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.ItemSystem {
	public struct ItemInteraction  : IInteractionContext {
		public Player player;
		public Level level;
		public ItemStack itemStack;
		public ItemInteractionTrigger trigger;

		public readonly Level GetLevel() => level;
	}
}
