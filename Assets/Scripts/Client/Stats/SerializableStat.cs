using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Unity.Plastic.Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SearchService;

#nullable enable

public class SerializableStat<TValue> : AbstractSerializableStat where TValue : struct, IComparable<TValue> {
	private static readonly Logger logger = Logger.CreateInstance();
	[JsonProperty] private StatDefinition<TValue> statDefinition;
	public TValue value { get; set; }
	public StatApplicationType applicationType { get; }
	public override bool showAsBonus { get; }
	public override bool persistent { get; set; }

	public SerializableStat(StatDefinition<TValue> statDefinition, TValue value, 
			StatApplicationType applicationType = StatApplicationType.Flat, 
			bool showAsBonus = false, 
			bool persistent = true) {
		this.statDefinition = statDefinition;
		this.value = value;
		this.applicationType = applicationType;
		this.showAsBonus = showAsBonus;
		this.persistent = persistent;
		
		if (applicationType == StatApplicationType.Percentage && typeof(TValue) == typeof(int)) {
			logger.LogWarning(null, "Unsupported stat application type 'percentage' for stat value type 'int'. Overriding with flat application type");
			this.applicationType = StatApplicationType.Flat;
		}
	}

	public override StatApplicationType GetApplicationType() => applicationType;

	public override object GetBoxedValue() => value;

	public override string GetFormattedExpression() => (statDefinition as IStatDefinitionImpl).GetFormattedExpression(value, showAsBonus);

	public override IStatDefinitionImpl GetStatDefinition() => statDefinition;

	public override string GetInfo() {
		return $"SerializableStat[type: {typeof(TValue)}, statDefinition: {statDefinition}, " +
			$"value: {value}, applicationType: {applicationType}," +
			$" showAsBonus: {showAsBonus}, persistent: {persistent}]";
	}

	internal override object Clone() {
		return new SerializableStat<TValue>(this.statDefinition, this.value, this.applicationType, this.showAsBonus, this.persistent);
	}
}

