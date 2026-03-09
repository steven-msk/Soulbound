using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.EntitySystem;
using SoulboundBackend.Common;
using SoulboundBackend.Core.AssetManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.ItemSystem {
	[PROTOTYPICAL]
	public sealed class SpawnEntityItem : Item, IItemAction {
		public override string name => "Spawn Entity Item";
		public override ItemAspect aspect => ItemAspect.Simple(new AssetKey("bluething"));

		public SpawnEntityItem() : base("spawnEntityItem") {
		}

		public bool ValidateTrigger(ItemActionTrigger trigger) {
			return trigger == ItemActionTrigger.LeftHold || trigger == ItemActionTrigger.LeftClick;
		}

		public bool CanExecute(ItemStack itemStack, Player player, Level level) {
			return true;
		}

		public bool TryExecute(ItemStack itemStack, Player player, Level level) {
			Entity entity = new PhysicsEntity(player.GetWorldPointerPos());
			level.AddEntity(entity);
			itemStack.Decrement();
			return true;
		}
	}
}
