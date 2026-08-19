using SoulboundEngine.Common;
using SoulboundEngine.Common.Math.Random;
using SoulboundEngine.Item;
using SoulboundEngine.Item.Container;
using SoulboundEngine.Loot.Context;
using SoulboundEngine.Loot.Function;
using SoulboundEngine.Registry;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SoulboundEngine.Loot {
	public class LootTable {
		private readonly List<LootPool> pools;
		private readonly IFunction<ItemStack, LootContext, ItemStack> combinedFunction;
		private readonly Identifier? randomSequenceId;

		public LootTable(List<LootPool> pools, List<ILootFunction> functions, Identifier? randomSequenceId) {
			this.pools = pools;
			this.randomSequenceId = randomSequenceId;
			this.combinedFunction = ILootFunction.Compile(functions);
		}

		public static Builder Create() => new();

		public ItemStack[] GenerateLoot(LootWorldContext worldContext) {
			return this.GenerateLoot(worldContext, worldContext.level.seed);
		}

		public ItemStack[] GenerateLoot(LootWorldContext worldContext, long seed) {
			List<ItemStack> stacks = new();
			this.GenerateLoot(worldContext, seed, stacks.Add);
			return stacks.ToArray();
		}

		public void GenerateLoot(LootWorldContext worldContext, long seed, Action<ItemStack> lootConsumer) {
			IRandom random = GetRandomSequence(worldContext, seed, this.randomSequenceId);
			this.GenerateLoot(new LootContext(worldContext, random), lootConsumer);
		}

		private static IRandom GetRandomSequence(LootWorldContext worldContext, long seed, Identifier? randomSequenceId) {
			return randomSequenceId is not null
				? worldContext.level.RandomSequences.GetOrCreate(randomSequenceId)
				: new Xoshiro256StarStarRandom(seed);
		}

		public ItemStack[] GenerateLoot(LootWorldContext worldContext, IRandom random) {
			return this.GenerateLoot(new LootContext(worldContext, random));
		}

		private ItemStack[] GenerateLoot(LootContext context) {
			List<ItemStack> stacks = new();
			this.GenerateLoot(context, stacks.Add);
			return stacks.ToArray();
		}

		public void GenerateLoot(LootContext context, Action<ItemStack> lootConsumer) {
			lootConsumer = ILootFunction.Apply(this.combinedFunction, lootConsumer, context);

			foreach (var pool in this.pools) {
				pool.AddGeneratedLoot(lootConsumer, context);
			}
		}

		public void SupplyInventory(IInventory inventory, LootWorldContext context, long seed) {
			inventory.Clear();
			IRandom random = GetRandomSequence(context, seed, this.randomSequenceId);
			ItemStack[] stacks = this.GenerateLoot(context, random);
			this.SpreadStacks(stacks, random, inventory.GetFreeSlots());
		}

		private void SpreadStacks(ItemStack[] stacks, IRandom random, IEnumerable<IItemSlot> slots) {
			static void Shuffle<T>(List<T> list, IRandom random) {
				for (int i = list.Count - 1; i > 0; i--) {
					int j = random.NextInt(0, i + 1);
					(list[i], list[j]) = (list[j], list[i]);
				}
			}
			static List<ItemStack> SplitToFillSlots(ItemStack[] stacks, int slotCount, IRandom random) {
				List<ItemStack> finalized = new();
				List<ItemStack> splittable = new();

				foreach (var stack in stacks) {
					if (stack.IsEmpty()) continue;
					if (stack.count > 1) splittable.Add(stack);
					else finalized.Add(stack);
				}

				while (slotCount - finalized.Count - splittable.Count > 0 && splittable.Count > 0) {
					int pickIndex = random.NextInt(0, splittable.Count);
					ItemStack toSplit = splittable[pickIndex];
					splittable.RemoveAt(pickIndex);

					int splitAmount = random.NextInt(1, toSplit.count / 2 + 1);
					ItemStack removed = toSplit.Split(splitAmount);

					if (toSplit.count > 1 && random.NextBool()) splittable.Add(toSplit);
					else finalized.Add(toSplit);

					if (removed.count > 1 && random.NextBool()) splittable.Add(removed);
					else finalized.Add(removed);
				}

				finalized.AddRange(splittable);
				return finalized;
			}

			List<IItemSlot> slotList = slots.ToList();
			List<ItemStack> spread = SplitToFillSlots(stacks, slotList.Count, random);

			Shuffle(spread, random);
			Shuffle(slotList, random);

			int count = Math.Min(spread.Count, slotList.Count);
			for (int i = 0; i < count; i++) {
				slotList[i].SetStack(spread[i]);
			}
		}

		public class Builder : ILootFunctionConsumingBuilder<Builder> {
			private readonly List<LootPool> pools = new();
			private readonly List<ILootFunction> functions = new();
			private Identifier? randomSequenceId;

			public LootTable Build() => new(this.pools, this.functions, this.randomSequenceId);

			public Builder Apply(ILootFunction.IBuilder function) {
				this.functions.Add(function.Build());
				return this.GetThis();
			}

			public Builder Pool(LootPool.Builder pool) {
				this.pools.Add(pool.Build());
				return this.GetThis();
			}

			public Builder RandomSequenceId(Identifier randomSequenceId) {
				this.randomSequenceId = randomSequenceId;
				return this.GetThis();
			}

			public Builder GetThis() => this;
		}
	}
}
