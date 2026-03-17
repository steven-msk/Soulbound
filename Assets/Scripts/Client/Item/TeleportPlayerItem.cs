using SoulboundBackend.Client.World;
using SoulboundBackend.Common;
using SoulboundBackend.Core.AssetManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.ItemSystem {
	[PROTOTYPICAL]
	public sealed class TeleportPlayerItem : Item, IItemInteractionListener {
		public override string name => "Move Player Item";
		public override int fullStackSize => 1;

		public override ItemAspect aspect => ItemAspect.Simple(new AssetKey("bluething"));

		public TeleportPlayerItem() : base("teleportPlayerItem") {
		}

		public bool ValidateTrigger(InteractionTrigger trigger) {
			return trigger == InteractionTrigger.LeftClick;
		}

		public bool CanExecute(ItemStack itemStack, in ItemInteraction ctx) {
			return true;
		}

		public bool TryExecute(ItemStack itemStack, in ItemInteraction ctx) {
			ctx.player.SetPos(ctx.player.GetWorldPointerPos());
			return true;
		}
	}
}
