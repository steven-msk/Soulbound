using SoulboundEngine.Client.Loot.Context;
using System;

namespace SoulboundEngine.Client.Loot.Provider.Number {
	public interface ILootNumberProvider {
		float NextFloat(LootContext context);
		int NextInt(LootContext context) => (int)Math.Floor(this.NextFloat(context));
	}
}
