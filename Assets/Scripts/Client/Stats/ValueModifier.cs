using SoulboundBackend.Common.Logging;
using System;
using Unity.Plastic.Newtonsoft.Json;

#nullable enable

namespace SoulboundBackend.Client.Stats {
	public class ValueModifier<TValue> : AbstractSerializableStat where TValue : struct, IComparable<TValue> {
		private static readonly Logger logger = Logger.CreateInstance();
		[JsonProperty] private StatDefinition<TValue> statDefinition;
		public TValue value { get; set; }
		public StatApplicationType applicationType { get; }
		public override bool showAsBonus { get; }
		public override bool persistent { get; set; }
		//public Func<TValue, string, string> displayNameFormat { get; }
		//public Func<TValue, string> valueFormat { get; }
		//public BonusAdmission<TValue> bonusValueAdmission { get; }
		//public Func<TValue, string>? valueColorSupplier { get; }

		public ValueModifier(
			StatDefinition<TValue> statDefinition,
			TValue value,
			//Func<TValue, string, string> displayNameFormat,
			//Func<TValue, string> valueFormat,
			//Func<TValue, string> valueColorSupplier,
			//BonusAdmission<TValue> bonusValueAdmission,
			StatApplicationType applicationType = StatApplicationType.Flat,
			bool showAsBonus = false,
			bool persistent = true
		) {
			this.statDefinition = statDefinition;
			this.value = value;
			this.applicationType = applicationType;
			this.showAsBonus = showAsBonus;
			this.persistent = persistent;
			//this.displayNameFormat = displayNameFormat;
			//this.valueFormat = valueFormat;
			//this.bonusValueAdmission = bonusValueAdmission;
			//this.valueColorSupplier = valueColorSupplier;

			if (applicationType == StatApplicationType.Percentage && typeof(TValue) == typeof(int)) {
				logger.LogWarning("Unsupported stat application type 'percentage' for stat value type 'int'. Overriding with flat application type");
				this.applicationType = StatApplicationType.Flat;
			}
		}

		public override StatApplicationType GetApplicationType() => applicationType;

		public override object GetBoxedValue() => value;

		//public override string GetFormattedExpression() => (statDefinition as IStatDefinition).GetFormattedExpression(value, showAsBonus);

		public override IStatDefinition GetStatDefinition() => statDefinition;

		public override string ToString() {
			return $"SerializableStat[type: {typeof(TValue)}, statDefinition: {statDefinition}, " +
				$"value: {value}, applicationType: {applicationType}," +
				$" showAsBonus: {showAsBonus}, persistent: {persistent}]";
		}

		internal override object Clone() {
			return new ValueModifier<TValue>(
				this.statDefinition,
				this.value, 
				//this.displayNameFormat,
				//this.valueFormat,
				//this.valueColorSupplier,
				//this.bonusValueAdmission,
				this.applicationType, 
				this.showAsBonus, 
				this.persistent);
		}
	}
}
