namespace SoulboundEngine.Loot.Condition {
	using SoulboundEngine.Common;
	using SoulboundEngine.Loot.Context;
	using System.Collections.Generic;
	using System.Linq;

	public class AllOfLootCondition : AlternativeLootCondition {
		public AllOfLootCondition(List<ILootCondition> conditions) 
			: base(conditions, conditions
				  .Cast<IPredicate<LootContext>>()
				  .Aggregate((a, b) => a.And(b))) {
		}

		public static Builder Create(ILootCondition.IBuilder[] conditions) {
			return new Builder(conditions);
		}

		public new class Builder : AlternativeLootCondition.Builder {
			public Builder(ILootCondition.IBuilder[] conditions) 
				: base(conditions) {
			}

			public override Builder And(ILootCondition.IBuilder builder) {
				this.Add(builder);
				return this;
			}

			protected override ILootCondition Build(List<ILootCondition> conditions) {
				return new AllOfLootCondition(conditions);
			}
		}
	}
}
