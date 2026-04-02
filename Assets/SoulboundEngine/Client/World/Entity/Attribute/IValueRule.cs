using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public interface IValueRule {
		void Apply(ref object value);
	}

	public interface IValueRule<T> : IValueRule {
		void Apply(ref T value);

		void IValueRule.Apply(ref object value) {
			if (value is not T wrap) {
				throw new InvalidOperationException($"Constraint expected {typeof(T).Name} but received {value?.GetType().Name ?? "null"}");
			}
			Apply(ref wrap);
			value = wrap;
		}
	}

	public record NumberRange(float minIncluded, float maxIncluded) : IValueRule<float> {
		public void Apply(ref float value) {
			if (value < minIncluded) value = minIncluded;
			if (value > maxIncluded) value = maxIncluded;
		}
	}

	public record PredicateValidator<T>(Predicate<T> predicate) : IValueRule<T> {
		public void Apply(ref T value) {
			if (!predicate(value)) throw new AttributeValueRuleViolationException($"Value does not meet condition: {value}");
		}
	}

	public record SetValidator<T>(T[] supportedValues, IEqualityComparer<T>? comparer = null) : IValueRule<T> {
		public void Apply(ref T value) {
			if (!supportedValues.Contains(value, comparer ?? EqualityComparer<T>.Default)) {
				throw new AttributeValueRuleViolationException($"Value is not within set: {value}");
			}
		}
	}
}
