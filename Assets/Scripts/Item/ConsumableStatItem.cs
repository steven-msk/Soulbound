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
    public override bool applyInstantStatsOnHoverOrSelect => false;
    public abstract IConsumable.ConsumeAction consumeAction { get; }
    public abstract int consumeAmount { get; }

    public virtual void Consume(ItemStack itemStack, PlayerController player) {
		ConsumableUtils.DefaultConsume(this, itemStack);
		((IStatProvider)this).ApplyInstantStats(player.Stats);
	}
}
