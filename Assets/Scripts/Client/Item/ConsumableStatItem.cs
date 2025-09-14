using SoulboundBackend.Client.Stats;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem {
	public abstract class ConsumableStatItem : StatItem, IConsumable {
		public event Action<IStatReceiver>? onConsumed;
		public override bool applyInstantStatsOnHoverOrSelect => false;
		public abstract IConsumable.ConsumeAction consumeAction { get; }
		public virtual int consumeAmount { get; } = 1;
		public abstract IConsumptionRestriction restriction { get; }

		public virtual void Consume(ItemStack itemStack) {
			if ((this as IConsumable).CanConsume(itemStack)) {
				onConsumed?.Invoke(GameManager.instance.Player.Stats);
			}
			Consumables.DefaultConsume(this, itemStack);
		}
	}
}
