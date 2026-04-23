using SoulboundEngine.Client.Interaction;
using SoulboundEngine.Client.ItemSystem.Render;
using SoulboundEngine.Client.World.EntitySystem;
using SoulboundEngine.Common;

namespace SoulboundEngine.Client.ItemSystem {
	[PROTOTYPICAL]
	public sealed class SpawnEntityItem : Item, IItemInteractionListener {
		public override string name => "Spawn Entity Item";

		public bool ValidateTrigger(InteractionTrigger trigger) {
			return trigger == InteractionTrigger.LeftHold || trigger == InteractionTrigger.LeftClick;
		}

		public bool CanExecute(ItemStack itemStack, in ItemInteraction ctx) {
			return true;
		}

		public bool TryExecute(ItemStack itemStack, in ItemInteraction ctx) {
			Entity entity = new PhysicsEntity(ctx.level);
			entity.SetPos(ctx.player.GetWorldPointerPos());
			ctx.level.AddEntity(entity);
			itemStack.Decrement();
			return true;
		}

		public override ItemRenderData GetRenderData(ItemStack itemStack) {
			return new ItemRenderData("bluething", itemStack);
		}
	}
}
