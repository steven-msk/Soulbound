using System;
using System.Collections.Generic;

public static class ItemAspectRegistry {
	private static readonly Dictionary<RuntimeTypeHandle, ItemAspect> mappings = new();

	public static ItemAspect Get(Item item, ItemAspect @default) {
		var handle = item.GetType().TypeHandle;
		if (!mappings.TryGetValue(handle, out var aspect)) {
			aspect = @default;
			mappings[handle] = aspect;
		}
		return aspect;
	}
}
