using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

#nullable enable

public class ConsumableStatItem : StatItemDefinition, IConsumable {
    public override bool applyInstantStatsAutomatically => false;
    public IConsumable.ConsumeAction consumeAction { get; }
    public int consumeAmount { get; }

    public ConsumableStatItem(string name, Sprite icon, Func<GameObject> worldPrefabSupplier, int maxStackSize, Func<Item, TooltipData?> tooltipSupplier, 
            List<AbstractSerializableStat> instantStats, List<IBufferedStatImpl> bufferedStats, string interpolationSource,
            IConsumable.ConsumeAction consumeAction, int consumeAmount, TooltipRenderer.NodeStyleProvider? nodeStyleProvider = null)
        : base(name, icon, worldPrefabSupplier, maxStackSize, tooltipSupplier, instantStats, bufferedStats, interpolationSource, nodeStyleProvider) {
        this.consumeAction = consumeAction;
        this.consumeAmount = consumeAmount;
    }


    public void Consume(ItemStack itemStack, PlayerController player) {
		ConsumableUtils.DefaultConsume(this, itemStack);
		((IStatProvider)this).ApplyInstantStats(player.Stats);
	}
}
