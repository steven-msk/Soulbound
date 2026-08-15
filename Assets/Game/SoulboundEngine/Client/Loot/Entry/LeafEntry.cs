using SoulboundEngine.Item;
using SoulboundEngine.Client.Loot.Condition;
using SoulboundEngine.Client.Loot.Context;
using SoulboundEngine.Client.Loot.Function;
using SoulboundEngine.Common;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SoulboundEngine.Client.Loot.Entry {
	public abstract class LeafEntry : LootPoolEntry, ILootChoice {
		public const int DEFAULT_QUALITY = 0;
		public const int DEFAULT_WEIGHT = 1;
		protected readonly IFunction<ItemStack, LootContext, ItemStack> compiledFunctions;
		protected readonly List<ILootFunction> functions;
		protected readonly int quality;
		protected readonly int weight;

		protected LeafEntry(int weight, int quality, List<ILootCondition> conditions, List<ILootFunction> functions) 
			: base(conditions) {
			this.weight = weight;
			this.quality = quality;
			this.functions = functions;
			this.compiledFunctions = ILootFunction.Compile(functions);
		}

		protected LeafEntry(List<ILootCondition> conditions, List<ILootFunction> functions) 
			: this(DEFAULT_WEIGHT, DEFAULT_QUALITY, conditions, functions) {
		}

		public static Builder<BasicBuilder> Create(Factory factory) => new BasicBuilder(factory);

		protected abstract void GenerateLoot(Action<ItemStack> lootConsumer, LootContext context);

		public override bool Expand(LootContext context, Action<ILootChoice> choiceConsumer) {
			if (!this.Test(context)) return false;

			choiceConsumer(this);
			return true;
		}

		protected virtual int GetWeight(float luck) {
			return Math.Max((int)MathF.Floor(this.weight + this.quality * luck), 0);
		}

		void ILootChoice.GenerateLoot(Action<ItemStack> lootConsumer, LootContext context) {
			this.GenerateLoot(lootConsumer, context);
		}

		int ILootChoice.GetWeight(float luck) {
			return this.GetWeight(luck);
		}

		public delegate LeafEntry Factory(int weight, int quality, List<ILootCondition> conditions, List<ILootFunction> functions);

		public new abstract class Builder<T> : LootPoolEntry.Builder<T>, ILootFunctionConsumingBuilder<T> where T : Builder<T> {
			private readonly List<ILootFunction> functions = new();
			protected int quality = DEFAULT_QUALITY;
			protected int weight = DEFAULT_WEIGHT;

			public T Apply(ILootFunction.IBuilder builder) {
				this.functions.Add(builder.Build());
				return this.GetThis();
			}

			protected List<ILootFunction> GetLootFunctions() => this.functions.ToList();

			public T Quality(int quality) {
				this.quality = quality;
				return this.GetThis();
			}

			public T Weight(int weight) {
				this.weight = weight;
				return this.GetThis();
			}

			T ILootFunctionConsumingBuilder<T>.GetThis() => this.GetThis();
		}

		public class BasicBuilder : Builder<BasicBuilder> {
			private readonly Factory factory;

			public BasicBuilder(Factory factory) {
				this.factory = factory;
			}

			public override LootPoolEntry Build() {
				return this.factory(this.weight, this.quality, this.GetConditions(), this.GetLootFunctions());
			}

			protected override BasicBuilder GetThis() => this;
		}
	}
}
