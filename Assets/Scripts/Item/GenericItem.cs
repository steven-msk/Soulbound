using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable

public class GenericItem : Item {
    public override string name { get; }
    public override Sprite icon { get; }
    public override Func<GameObject> worldPrefabSupplier { get; }
    public override int maxStackSize { get; }
    protected override Func<Item, TooltipData?> tooltipSupplier { get; }
    protected override TooltipRenderer.NodeStyleProvider? nodeStyleProvider { get; }

    public GenericItem(string name, Sprite icon, Func<GameObject> worldPrefabSupplier, int maxStackSize, Func<Item, TooltipData?> tooltipSupplier,
                       TooltipRenderer.NodeStyleProvider? nodeStyleProvider = null) {
        this.name = name;
        this.icon = icon;
        this.worldPrefabSupplier = worldPrefabSupplier;
        this.maxStackSize = maxStackSize;
        this.tooltipSupplier = tooltipSupplier;
        this.nodeStyleProvider = nodeStyleProvider;
    }
}
