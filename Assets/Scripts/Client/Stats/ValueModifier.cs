using SoulboundBackend.Common.Logging;
using System;
using Unity.Plastic.Newtonsoft.Json;

#nullable enable

namespace SoulboundBackend.Client.Stats {
	public class ValueModifier<TValue> : AbstractValueModifier, IStatEntryModifier<TValue> 
			where TValue : struct, IComparable<TValue> {
		private static readonly Logger logger = Logger.CreateInstance();
		public readonly TValue value;
		public override bool keepSign { get; }

		public ValueModifier(
			StatDefinition<TValue> statDefinition,
			TValue value,
			bool keepSign,
			StatApplicationType applicationType = StatApplicationType.Flat
		) 
			: base(statDefinition, applicationType) {
			this.value = value;
			this.keepSign = keepSign;

			if (applicationType == StatApplicationType.Percentage && typeof(TValue) == typeof(int)) {
				logger.LogWarning("Unsupported stat application type 'percentage' for stat value type 'int'.");
			}
		}

		public virtual void Apply(StatEntry<TValue> entry, ModificationToken modificationToken) {
			throw new NotImplementedException();
		}

		public virtual void Remove(StatEntry<TValue> entry, ModificationToken modificationToken) {
			throw new NotImplementedException();
		}

		public override object GetBoxedValue() => value;

		public override string ToString() {
			return $"ValueModifier[type: {typeof(TValue)}, " +
				   $"statDefinition: {statDefinition}, " +
				   $"value: {value}, " +
				   $"applicationType: {applicationType}, " +
				   $"keepSign: {keepSign}]";
		}

		public override object Clone() {
			return new ValueModifier<TValue>(
				(StatDefinition<TValue>)this.statDefinition,
				this.value, 
				this.keepSign,
				this.applicationType
			);
		}

		public override int GetHashCode() {
			return HashCode.Combine(statDefinition, value, keepSign, applicationType);
		}
	}
}
