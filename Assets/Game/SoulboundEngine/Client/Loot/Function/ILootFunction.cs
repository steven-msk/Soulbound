using SoulboundEngine.Client.Item;
using SoulboundEngine.Client.Loot.Context;
using SoulboundEngine.Common;
using System;
using System.Collections.Generic;

namespace SoulboundEngine.Client.Loot.Function {
	public interface ILootFunction : IFunction<ItemStack, LootContext, ItemStack> {
		public static Action<ItemStack> Apply(IFunction<ItemStack, LootContext, ItemStack> itemApplier, Action<ItemStack> lootConsumer, LootContext context) {
			return stack => lootConsumer(itemApplier.Apply(stack, context));
		}

		public static IFunction<ItemStack, LootContext, ItemStack> Compile(List<ILootFunction> functions) {
			return Of((item, lootContext) => {
				ItemStack stack = item;
				foreach (var function in functions) {
					stack = function.Apply(stack, lootContext);
				}
				return stack;
			});
		}

		public interface IBuilder {
			ILootFunction Build();

			public static IBuilder Of(Func<ILootFunction> func) {
				return new DelegateImpl(func);
			}

			private sealed class DelegateImpl : IBuilder {
				private readonly Func<ILootFunction> func;

				public DelegateImpl(Func<ILootFunction> func) {
					this.func = func;
				}

				public ILootFunction Build() => this.func();
			}
		}
	}
}
