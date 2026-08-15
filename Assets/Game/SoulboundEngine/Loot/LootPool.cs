using SoulboundEngine.Item;
using SoulboundEngine.Loot.Condition;
using SoulboundEngine.Loot.Context;
using SoulboundEngine.Loot.Entry;
using SoulboundEngine.Loot.Function;
using SoulboundEngine.Loot.Provider.Number;
using SoulboundEngine.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoulboundEngine.Loot {
	public class LootPool {
		private readonly ILootNumberProvider rolls;
		private readonly List<LootPoolEntry> entries;
		private readonly Predicate<LootContext> predicate;
		private readonly IFunction<ItemStack, LootContext, ItemStack> compiledFunctions;
			
		public LootPool(List<LootPoolEntry> entries, List<ILootCondition> conditions, List<ILootFunction> functions, ILootNumberProvider rolls) {
			this.rolls = rolls;
			this.entries = entries;
			this.compiledFunctions = ILootFunction.Compile(functions);
			this.predicate = context => conditions.All(c => c.Test(context));
		}

		public static Builder Create() => new();

		public void AddGeneratedLoot(Action<ItemStack> consumer, LootContext context) {
			if (!this.predicate.Invoke(context)) return;

			Action<ItemStack> decoratedConsumer = ILootFunction.Apply(this.compiledFunctions, consumer, context);

			List<ILootChoice> choices = new();
			foreach (var entry in this.entries) {
				entry.Expand(context, choices.Add);
			}
			if (choices.Count == 0) return;

			int totalWeight = choices.Sum(c => c.GetWeight(context.Luck));
			if (totalWeight <= 0) return;

			int rolls = this.rolls.NextInt(context);
			for (int i = 0; i < rolls; i++) {
				ILootChoice choice = GetRandomWeightedChoice(choices, context, totalWeight);
				choice.GenerateLoot(consumer, context);
			}
		}

		private static ILootChoice GetRandomWeightedChoice(List<ILootChoice> choices, LootContext context, int totalWeight) {
			int roll = context.random.NextInt(0, totalWeight);

			foreach (ILootChoice choice in choices) {
				roll -= choice.GetWeight(context.Luck);
				if (roll <= 0) return choice;
			}

			throw new InvalidOperationException("Failed to select a loot choice.");
		}

		public class Builder : ILootFunctionConsumingBuilder<Builder>, ILootConditionConsumingBuilder<Builder> {
			private readonly List<ILootCondition> conditions = new();
			private readonly List<ILootFunction> functions = new();
			private readonly List<LootPoolEntry> entries = new();
			private ILootNumberProvider rolls;

			public LootPool Build() => new(this.entries, this.conditions, this.functions, this.rolls);

			public Builder Apply(ILootFunction.IBuilder builder) {
				this.functions.Add(builder.Build());
				return this.GetThis();
			}

			public Builder Conditionally(ILootCondition.IBuilder condition) {
				this.conditions.Add(condition.Build());
				return this.GetThis();
			}

			public Builder Rolls(ILootNumberProvider rolls) {
				this.rolls = rolls;
				return this.GetThis();
			}

			public Builder With(LootPoolEntry.Builder entry) {
				this.entries.Add(entry.Build());
				return this.GetThis();
			}

			public Builder GetThis() => this;
		}
	}
}
