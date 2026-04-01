using System;
using System.Linq;

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public interface IValueRange {
		bool IsValid(object value);
	}

	public interface IValueRange<T> : IValueRange {
		bool IsValid(T value);

		bool IValueRange.IsValid(object value) {
			if (value is not T) return false;
			return IsValid((T)value);
		}
	}

	public sealed record NumberRange<T>(T minIncluded, T maxIncluded) : IValueRange<T> where T : struct, IComparable<T> {
		public bool IsValid(T value) {
			return value.CompareTo(minIncluded) >= 0 && value.CompareTo(maxIncluded) <= 0;
		}
	}

	public record SetRange<T>(params T[] supportedValues) : IValueRange<T> {
		public bool IsValid(T value) {
			return supportedValues.Contains(value);
		}
	}

	public record PredicateRange<T>(Predicate<T> predicate) : IValueRange<T> {
		public bool IsValid(T value) => predicate(value);
	}
}
