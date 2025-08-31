using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.UIElements;

#nullable enable

public static class ItemTooltips {
    public static Func<Item, Tooltip?> NoTooltip() => item => null;

    //public static Func<Item, AbstractTooltip?> DefaultTitle() => item => CompoundTooltip.Of(Tooltip.Title(item.name));
}
