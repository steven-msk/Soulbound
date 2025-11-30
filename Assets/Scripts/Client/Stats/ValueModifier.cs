using SoulboundBackend.Common.Logging;
using System;
using Unity.Plastic.Newtonsoft.Json;

#nullable enable

namespace SoulboundBackend.Client.Stats {
	public class ValueModifier<TValue> : AbstractValueModifier where TValue : struct, IComparable<TValue> {
		private static readonly Logger logger = Logger.CreateInstance();
		public TValue value { get; set; }
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

		public override object GetBoxedValue() => value;

		public override string ToString() {
			return $"SerializableStat[type: {typeof(TValue)}, statDefinition: {statDefinition}, " +
				$"value: {value}, applicationType: {applicationType}," +
				$" showAsBonus: {keepSign}]";
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
