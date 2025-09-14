using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem {
	public interface IConsumable : IItemCapability {
		public delegate void ConsumeAction(IConsumable consumable, ItemStack itemStack);

		public ConsumeAction? consumeAction { get; }
		public int consumeAmount { get; }
		public IConsumptionRestriction restriction { get; }

		public virtual ConsumptionResult Consume(ItemStack itemStack) {
			return Consumables.DefaultConsume(this, itemStack);
		}
	}

	public record ConsumptionResult(ConsumeMode mode);

	public static class Consumables {
		public static ConsumptionResult DefaultConsume(IConsumable consumable, ItemStack itemStack) {
			UnityEngine.Debug.Log("attempting to consume");
			ConsumptionDirective directive = consumable.restriction?.Evaluate(consumable, itemStack) ?? new(ConsumeMode.Allow);
			if (directive.mode == ConsumeMode.Block) {
				return new ConsumptionResult(ConsumeMode.Block);
			}
			consumable.consumeAction?.Invoke(consumable, itemStack);
			if (directive.mode == ConsumeMode.Override) {
				directive.customEffect?.Invoke(itemStack);
			}
			itemStack.Decrement(consumable.consumeAmount);
			consumable.restriction?.NotifyConsumed(itemStack);
			UnityEngine.Debug.Log("successfully consumed");
			return new ConsumptionResult(directive.mode);
		}
	}

}
