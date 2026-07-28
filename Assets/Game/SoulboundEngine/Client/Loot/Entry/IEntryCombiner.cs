using SoulboundEngine.Client.Loot.Context;
using System;

namespace SoulboundEngine.Client.Loot.Entry {
	public interface IEntryCombiner {
		public static readonly IEntryCombiner ALWAYS_FALSE = new DelegateImpl((_, _) => false);
		public static readonly IEntryCombiner ALWAYS_TRUE = new DelegateImpl((_, _) => true);

		bool Expand(LootContext context, Action<ILootChoice> choiceConsumer);

		internal protected sealed class DelegateImpl : IEntryCombiner {
			private readonly Func<LootContext, Action<ILootChoice>, bool> func;

			public DelegateImpl(Func<LootContext, Action<ILootChoice>, bool> func) {
				this.func = func;
			}

			public bool Expand(LootContext context, Action<ILootChoice> choiceConsumer) {
				return this.func(context, choiceConsumer);
			}
		}
	}

	public static class EntryCombinerExtensions {
		public static IEntryCombiner And(this IEntryCombiner combiner, IEntryCombiner other) {
			return new IEntryCombiner.DelegateImpl((context, choiceConsumer) => { 
				return combiner.Expand(context, choiceConsumer) && other.Expand(context, choiceConsumer);
			});
		}

		public static IEntryCombiner Or(this IEntryCombiner combiner, IEntryCombiner other) {
			return new IEntryCombiner.DelegateImpl((context, choiceConsumer) => {
				// || operator short-circuits, so caching results avoids that
				// this assumes that the second expansion always runs, regardless of whether the first one failed or not
				bool a = combiner.Expand(context, choiceConsumer);
				bool b = other.Expand(context, choiceConsumer);
				return a || b;
			});
		}
	}
}
