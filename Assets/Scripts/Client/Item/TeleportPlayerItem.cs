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
	public sealed class TeleportPlayerItem : Item, IItemAction {
		public override string name => "Move Player Item";
		public override int fullStackSize => 1;

		public override ItemAspect aspect => ItemAspect.Simple(new AssetKey("bluething"));

		public TeleportPlayerItem() : base("teleportPlayerItem") {
		}

		public bool ValidateTrigger(ItemActionTrigger trigger) {
			return trigger == ItemActionTrigger.LeftClick;
		}

		public bool CanExecute(ItemStack itemStack, Player player, Level level) {
			return true;
		}

		public bool TryExecute(ItemStack itemStack, Player player, Level level) {
			player.SetPos(player.GetWorldPointerPos());
			return true;
		}
	}
}
