using System;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

#nullable enable

public class ConsumableItem : ItemDefinition, IConsumable {
    public IConsumable.ConsumeAction consumeAction { get; }
    public virtual int consumeAmount { get; }

    public ConsumableItem(string name, Sprite icon, Func<GameObject> worldPrefabSupplier, int maxStackSize, Func<Item, TooltipData?> tooltipSupplier,
            IConsumable.ConsumeAction consumeAction, int consumeAmount)
        : base(name, icon, worldPrefabSupplier, maxStackSize, tooltipSupplier) {
        this.consumeAction = consumeAction;
        this.consumeAmount = consumeAmount;
    }

    //protected override CompoundTooltip GetDefaultTooltip() => base.GetDefaultTooltip().Concat(Tooltip.Tag(ItemTag.Consumable));
}
