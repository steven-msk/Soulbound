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

		public virtual bool CanConsume(ItemStack itemStack) {
			return restriction == null || restriction.CanConsume(this, itemStack);
		}

		public virtual void Consume(ItemStack itemStack) {
			Consumables.DefaultConsume(this, itemStack);
		}
	}

	public static class Consumables {
		public static void DefaultConsume(IConsumable consumable, ItemStack itemStack) {
			UnityEngine.Debug.Log("attempting to consume");
			if (consumable.CanConsume(itemStack)) {
				consumable.consumeAction?.Invoke(consumable, itemStack);
				itemStack.Decrement(consumable.consumeAmount);
				consumable.restriction.NotifyConsumed(itemStack);
				UnityEngine.Debug.Log("successfully consumed");
			}
		}
	}

}
