using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.UIElements;

#nullable enable

public static class ItemTooltips {
    public static Func<Item, AbstractTooltip?> NoTooltip() => item => null;

    public static Func<Item, AbstractTooltip?> DefaultTitle() => item => Tooltip.Title(item.name);
}
