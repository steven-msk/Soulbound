namespace SoulboundEngine.Loot.Entry {
	using SoulboundEngine.Common;
	using SoulboundEngine.Loot.Condition;
	using SoulboundEngine.Loot.Context;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public abstract class LootPoolEntry : IEntryCombiner {
		private readonly IPredicate<LootContext> conditionPredicate;
		protected readonly List<ILootCondition> conditions;

		protected LootPoolEntry(List<ILootCondition> conditions) {
			this.conditions = conditions;
			this.conditionPredicate = IPredicate<LootContext>.Of(context => {
				return this.conditions.All(c => c.Test(context));
			});
		}

		protected bool Test(LootContext context) => this.conditionPredicate.Test(context);

		public abstract bool Expand(LootContext context, Action<ILootChoice> choiceConsumer);

		public abstract class Builder {
			public abstract LootPoolEntry Build();
		}

		public abstract class Builder<T> : Builder,  ILootConditionConsumingBuilder<T> where T : Builder<T> {
			private readonly List<ILootCondition> conditions = new();

			protected abstract T GetThis();

			public T Conditionally(ILootCondition.IBuilder builder) {
				this.conditions.Add(builder.Build());
				return this.GetThis();
			}

			protected List<ILootCondition> GetConditions() => this.conditions;

			T ILootConditionConsumingBuilder<T>.GetThis() => this.GetThis();
		}
	}
}
