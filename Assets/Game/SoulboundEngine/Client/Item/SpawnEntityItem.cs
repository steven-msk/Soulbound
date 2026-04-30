using SoulboundEngine.Client.Interaction;
using SoulboundEngine.Client.World.EntitySystem;
using SoulboundEngine.Common;

namespace SoulboundEngine.Client.ItemSystem {
	[PROTOTYPICAL]
	public sealed class SpawnEntityItem : Item, IItemInteractionListener {
		public SpawnEntityItem(Settings settings) : base(settings) {
		}

		public bool ValidateTrigger(InteractionTrigger trigger) {
			return trigger == InteractionTrigger.LeftHold || trigger == InteractionTrigger.LeftClick;
		}

		public bool CanExecute(ItemStack itemStack, in ItemInteraction ctx) {
			return true;
		}

		public bool TryExecute(ItemStack itemStack, in ItemInteraction ctx) {
			EntityType.PHYSICS_ENTITY.Create(ctx.level, ctx.player.GetWorldPointerPos());
			itemStack.Decrement();
			return true;
		}
	}
}
