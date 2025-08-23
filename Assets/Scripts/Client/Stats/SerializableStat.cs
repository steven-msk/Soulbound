using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SearchService;

#nullable enable

public class SerializableStat<TValue> : AbstractSerializableStat where TValue : struct, IComparable<TValue> {
	public StatDefinition<TValue> statType;
	public TValue value;
	public StatApplicationType applicationType;
	public override bool applyAsBonus { get; }

	public SerializableStat(StatDefinition<TValue> statType, TValue value, StatApplicationType appliance, bool applyAsBonus) {
		this.statType = statType;
		this.value = value;
		this.applicationType = appliance;
		this.applyAsBonus = applyAsBonus;
	}

	public override StatApplicationType GetApplicationType() => applicationType;

	public override object GetBoxedValue() => value;

	public override string GetFormattedExpression() => (statType as IStatDefinitionImpl).GetFormattedExpression(value, applyAsBonus);

	public override IStatDefinitionImpl GetStatDefinition() => statType;
}

