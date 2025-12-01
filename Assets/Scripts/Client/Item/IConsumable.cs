using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem {
	public interface IConsumable : IItemCapability {
		public int consumeAmount { get; }
		ConsumptionResult Consume(IItemConsumer consumer, ItemStack itemStack);

		public void StartConsume(IItemConsumer consumer, ItemStack itemStack) {
			var result = Consume(consumer, itemStack);
			if (result.success) {
				itemStack.Decrement(consumeAmount);
			}
		}
	}
}
