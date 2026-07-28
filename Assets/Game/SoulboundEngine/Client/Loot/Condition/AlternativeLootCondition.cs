using Game.SoulboundEngine.Common;
using SoulboundEngine.Client.Loot.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoulboundEngine.Client.Loot.Condition {
	public abstract class AlternativeLootCondition : ILootCondition {
		protected readonly List<ILootCondition> conditions;
		private readonly IPredicate<LootContext> predicate;

		protected AlternativeLootCondition(List<ILootCondition> conditions, IPredicate<LootContext> predicate) {
			this.conditions = conditions;
			this.predicate = predicate;
		}

		public bool Test(LootContext context) {
			return this.predicate.Test(context);
		}

		public abstract class Builder : ILootCondition.IBuilder {
			private readonly List<ILootCondition> conditions = new();

			protected Builder(ILootCondition.IBuilder[] conditions) {
				this.conditions.AddRange(conditions.Select(c => c.Build()));
			}

			public void Add(ILootCondition.IBuilder builder) {
				this.conditions.Add(builder.Build());
			}

			public virtual AllOfLootCondition.Builder And(ILootCondition.IBuilder builder) {
				return AllOfLootCondition.Create(new[] { this, builder });
			}

			public virtual AnyOfLootCondition.Builder Or(ILootCondition.IBuilder builder) {
				return AnyOfLootCondition.Create(new[] { this, builder });
			}

			public ILootCondition Build() {
				if (this.conditions.Count == 0) {
					throw new ArgumentException("AlternativeLootCondition requires at least one condition");
				}
				return this.Build(this.conditions);
			}

			protected abstract ILootCondition Build(List<ILootCondition> conditions);
		}
	}
}
