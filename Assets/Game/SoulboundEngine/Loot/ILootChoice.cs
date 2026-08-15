using SoulboundEngine.Item;
using SoulboundEngine.Loot.Context;
using System;

namespace SoulboundEngine.Loot {
	public interface ILootChoice {
		void GenerateLoot(Action<ItemStack> lootConsumer, LootContext context);
		int GetWeight(float luck);
	}
}
