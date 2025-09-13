using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

#nullable enable

public abstract class ConsumableStatItem : StatItem, IConsumable {
    public event Action<IStatReceiver>? onConsumed;
    public override bool applyInstantStatsOnHoverOrSelect => false;
    public abstract IConsumable.ConsumeAction consumeAction { get; }
    public virtual int consumeAmount { get; } = 1;
	public abstract IEnumerable<IConsumptionRestriction>? restrictions { get; }

	public virtual void Consume(ItemStack itemStack) {
        if ((this as IConsumable).CanConsume(itemStack)) {
            onConsumed?.Invoke(GameManager.instance.Player.Stats);
        }
		Consumables.DefaultConsume(this, itemStack);
	}
}
