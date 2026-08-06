using SoulboundEngine.Client.Item;
using SoulboundEngine.Client.World.Entity;
using SoulboundEngine.Client.World.Level;
using System;

namespace SoulboundEngine.Client.Interaction {
	public record ActiveUseContext(ItemStack stack, InteractionType type, Level level, Entity user, int useTime, int remainingTicks) {
		public ActiveUseContext Tick(Func<ItemStack, ActiveUseContext> onFinish, Action<ItemStack> tickConsumer) {
			ItemStack tickedStack = this.stack.OnUseTick(this.type, this.level, this.user, this.remainingTicks);
			tickConsumer(tickedStack);
			ActiveUseContext newContext = new(tickedStack, this.type, this.level, this.user, this.useTime, this.remainingTicks - 1);

			bool finished = newContext.remainingTicks <= 0;
			if (finished) {
				return onFinish(tickedStack.OnUseFinished(this.type, this.level, this.user));
			}

			return newContext;
		}

		public void Cancel(Action<ItemStack> consumer) {
			consumer(this.stack.OnUseCanceled(this.type, this.level, this.user, this.remainingTicks));
		}
	}
}
