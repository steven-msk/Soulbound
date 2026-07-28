using Game.SoulboundEngine.Common;
using SoulboundEngine.Client.Loot.Context;

namespace SoulboundEngine.Client.Loot.Condition {
	public interface ILootCondition : IPredicate<LootContext> {
		public interface IBuilder {
			ILootCondition Build();

			AllOfLootCondition.Builder And(IBuilder condition);
			AnyOfLootCondition.Builder Or(IBuilder builder);
		}
	}
}
