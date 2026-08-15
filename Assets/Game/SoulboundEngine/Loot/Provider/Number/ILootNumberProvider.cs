using SoulboundEngine.Loot.Context;
using System;

namespace SoulboundEngine.Loot.Provider.Number {
	public interface ILootNumberProvider {
		float NextFloat(LootContext context);
		int NextInt(LootContext context) => (int)Math.Floor(this.NextFloat(context));
	}
}
