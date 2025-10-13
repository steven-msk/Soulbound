using System;
using System.Collections.Generic;

namespace SoulboundBackend.Client.ItemSystem {
	public static class ItemAspectRegistry {
		private static readonly Dictionary<RuntimeTypeHandle, ItemAspect> mappings = new();

		public static ItemAspect Get(Item item, Func<ItemAspect> @default) {
			var handle = item.GetType().TypeHandle;
			if (!mappings.TryGetValue(handle, out var aspect)) {
				aspect = @default.Invoke();
				mappings[handle] = aspect;
			}
			return aspect;
		}
	}
}
