using System;
using System.Collections.Generic;

namespace SoulboundEngine.Client.Loot.Condition {
	public interface ILootConditionConsumingBuilder<out T> where T : ILootConditionConsumingBuilder<T> {
		T Conditionally(ILootCondition.IBuilder condition);

		T GetThis();
	}

	public static class ConditionConsumingBuilderExtensions {
		public static T Conditionally<T, E>(this ILootConditionConsumingBuilder<T> builder, IEnumerable<E> conditions, Func<E, ILootCondition.IBuilder> toBuilder) where T : ILootConditionConsumingBuilder<T> {
			foreach (var condition in conditions) {
				builder.Conditionally(toBuilder(condition));
			}
			return builder.GetThis();
		}
	}
}
