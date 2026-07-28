using Game.SoulboundEngine.Common;
using SoulboundEngine.Client.Loot.Context;
using System.Collections.Generic;
using System.Linq;

namespace SoulboundEngine.Client.Loot.Condition {
	public class AnyOfLootCondition : AlternativeLootCondition {
		public AnyOfLootCondition(List<ILootCondition> conditions) 
			: base(conditions, conditions
				  .Cast<IPredicate<LootContext>>()
				  .Aggregate((a, b) => a.Or(b))) {
		}

		public static Builder Create(ILootCondition.IBuilder[] conditions) {
			return new Builder(conditions);
		}

		public new class Builder : AlternativeLootCondition.Builder {
			public Builder(ILootCondition.IBuilder[] conditions) 
				: base(conditions) {
			}

			public override Builder Or(ILootCondition.IBuilder condition) {
				this.Add(condition);
				return this;
			}

			protected override ILootCondition Build(List<ILootCondition> conditions) {
				return new AnyOfLootCondition(conditions);
			}
		}
	}
}
