using Game.SoulboundEngine.Common;
using SoulboundEngine.Loot.Context;

namespace SoulboundEngine.Loot.Condition {
	public interface ILootCondition : IPredicate<LootContext> {
		public interface IBuilder {
			ILootCondition Build();

			AllOfLootCondition.Builder And(IBuilder condition);
			AnyOfLootCondition.Builder Or(IBuilder builder);
		}
	}
}
