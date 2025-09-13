using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class ItemAspectRegistry {
	private static readonly Dictionary<Item, ItemAspect> mappings = new();

	public static ItemAspect Get(Item item, ItemAspect @default) {
		if (!mappings.TryGetValue(item, out var aspect)) {
			aspect = @default;
			mappings[item] = aspect;
		}
		return aspect;
	}
}
