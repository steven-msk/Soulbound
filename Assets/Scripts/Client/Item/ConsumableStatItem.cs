using SoulboundBackend.Client.Stats;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem {
	public abstract class ConsumableStatItem : StatItem, IConsumable {
		public override bool applyInstantStatsOnHoverOrSelect => false;
		public virtual int consumeAmount { get; } = 1;

		public virtual ConsumptionResult Consume(IItemConsumer consumer, ItemStack itemStack) {
			consumer.statModificationHost?.ApplyModifiers(this);
			return ConsumptionResult.Success(itemStack);
		}
	}
}
