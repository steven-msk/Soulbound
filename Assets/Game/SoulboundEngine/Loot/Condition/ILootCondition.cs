namespace SoulboundEngine.Loot.Condition {
	using SoulboundEngine.Common;
	using SoulboundEngine.Loot.Context;

	public interface ILootCondition : IPredicate<LootContext> {
		public interface IBuilder {
			ILootCondition Build();

			AllOfLootCondition.Builder And(IBuilder condition);
			AnyOfLootCondition.Builder Or(IBuilder builder);
		}
	}
}
