using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.ItemSystem {
	public static class ItemRegistry {
		static readonly Dictionary<string, Item> itemsById = new();

		public static Item Register(Item item) {
			itemsById[item.GetID()] = item;
			return item;
		}

		public static bool TryGet(string id, out Item item) {
			return itemsById.TryGetValue(id, out item);
		}

		public static IEnumerable<Item> GetAll() => itemsById.Values;
	}
}
