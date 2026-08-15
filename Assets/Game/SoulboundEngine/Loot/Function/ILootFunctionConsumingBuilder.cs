using System;
using System.Collections.Generic;
using System.Linq;

namespace SoulboundEngine.Loot.Function {
	public interface ILootFunctionConsumingBuilder<out T> where T : ILootFunctionConsumingBuilder<T> {
		T Apply(ILootFunction.IBuilder builder);

		T GetThis();
	}

	public static class FunctionConsumingBuilderExtensions {
		public static T Apply<T, E>(this ILootFunctionConsumingBuilder<T> builder, E[] functions, Func<E, ILootFunction.IBuilder> toBuilder) where T : ILootFunctionConsumingBuilder<T> {
			return builder.Apply(functions.AsEnumerable(), toBuilder);
		}

		public static T Apply<T, E>(this ILootFunctionConsumingBuilder<T> builder, IEnumerable<E> functions, Func<E, ILootFunction.IBuilder> toBuilder) where T : ILootFunctionConsumingBuilder<T> {
			foreach (var function in functions) {
				builder.Apply(toBuilder(function));
			}
			return builder.GetThis();
		}
	}
}
