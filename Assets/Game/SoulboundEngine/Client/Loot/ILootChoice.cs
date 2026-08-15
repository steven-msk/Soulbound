using SoulboundEngine.Item;
using SoulboundEngine.Client.Loot.Context;
using System;

namespace SoulboundEngine.Client.Loot {
	public interface ILootChoice {
		void GenerateLoot(Action<ItemStack> lootConsumer, LootContext context);
		int GetWeight(float luck);
	}
}
