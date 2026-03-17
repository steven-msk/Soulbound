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
	public sealed class SpawnEntityItem : Item, IItemInteractionListener {
		public override string name => "Spawn Entity Item";
		public override ItemAspect aspect => ItemAspect.Simple(new AssetKey("bluething"));

		public SpawnEntityItem() : base("spawnEntityItem") {
		}

		public bool ValidateTrigger(InteractionTrigger trigger) {
			return trigger == InteractionTrigger.LeftHold || trigger == InteractionTrigger.LeftClick;
		}

		public bool CanExecute(ItemStack itemStack, in ItemInteraction ctx) {
			return true;
		}

		public bool TryExecute(ItemStack itemStack, in ItemInteraction ctx) {
			Entity entity = new PhysicsEntity(ctx.player.GetWorldPointerPos());
			ctx.level.AddEntity(entity);
			itemStack.Decrement();
			return true;
		}
	}
}
