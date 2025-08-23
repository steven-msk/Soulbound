using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

#nullable enable

public class StatDefinition<TValue> : IStatDefinitionImpl where TValue : struct, IComparable<TValue> {
	public static readonly StatDefinition<int> MaxHealth = new("Max Health",
		StatDisplayFormatter.PlainNameFormat<int>("#FF6B6B"),
		StatDisplayFormatter.PlainValueFormat<int>(),
		StatDisplayFormatter.ColorPositiveNegative<int>(),
		BonusAdmission<int>.AddAndSubtract,
		SupportedApplicationType.Flat,
		StatProcessors.Flat<int>());
	
	public static readonly StatDefinition<int> MaxMana = new("Max Mana",
		StatDisplayFormatter.PlainNameFormat<int>("#6BCBFF"),
		StatDisplayFormatter.PlainValueFormat<int>(), 
		StatDisplayFormatter.ColorPositiveNegative<int>(),
		BonusAdmission<int>.AddAndSubtract, 
		SupportedApplicationType.Flat,
		StatProcessors.Flat<int>());

	public static readonly StatDefinition<int> Defense = new("Defense",
		StatDisplayFormatter.PlainNameFormat<int>("#CCDDEE"),
		StatDisplayFormatter.PlainValueFormat<int>(),
		StatDisplayFormatter.ColorPositiveNegative<int>(),
		BonusAdmission<int>.AddAndSubtract,
		SupportedApplicationType.Flat,
		StatProcessors.Flat<int>());

	public static readonly StatDefinition<int> SoulSlots = new("Soul Slot",
		StatDisplayFormatter.PluralAdaptedNameFormat<int>("#c86bff"),
		StatDisplayFormatter.PlainValueFormat<int>(),
		StatDisplayFormatter.ColorPositiveNegative<int>(),
		BonusAdmission<int>.AddAndSubtract,
		SupportedApplicationType.Flat,
		StatProcessors.Flat<int>());

	public static readonly StatDefinition<float> MovementSpeed = new("Movement Speed",
		StatDisplayFormatter.PlainNameFormat<float>("#6BFFB6"),
		StatDisplayFormatter.PercentageValueFormat(),
		StatDisplayFormatter.ColorPositiveNegative<float>(),
		BonusAdmission<float>.AddAndSubtract,
		SupportedApplicationType.Percentage,
		StatProcessors.Percentage());

	public static readonly StatDefinition<int> JumpHeight = new("Jump Height",
		StatDisplayFormatter.PlainNameFormat<int>("#67E8F9"),
		StatDisplayFormatter.PlainValueFormat<int>(),
		StatDisplayFormatter.ColorPositiveNegative<int>(),
		BonusAdmission<int>.Add,
		SupportedApplicationType.Flat,
		StatProcessors.Flat<int>());

	public static readonly StatDefinition<int> MaxJumps = new("Max Jump",
		StatDisplayFormatter.PluralAdaptedNameFormat<int>("#67E8F9"),
		StatDisplayFormatter.PlainValueFormat<int>(),
		StatDisplayFormatter.ColorPositiveNegative<int>(),
		BonusAdmission<int>.Add,
		SupportedApplicationType.Flat,
		StatProcessors.Flat<int>());

	public static readonly StatDefinition<float> DashVelocity = new("Dash Velocity",
		StatDisplayFormatter.PlainNameFormat<float>("#5EEAD4"),
		StatDisplayFormatter.PercentageValueFormat(),
		StatDisplayFormatter.ColorPositiveNegative<float>(),
		BonusAdmission<float>.AddAndSubtract,
		SupportedApplicationType.Percentage,
		StatProcessors.Percentage());

	public static readonly StatDefinition<float> DashCooldown = new("Dash Cooldown",
		StatDisplayFormatter.PlainNameFormat<float>("#4BFFE0"),
		value => $"-{StatDisplayFormatter.PercentageValueFormat()(value)}",
		StatDisplayFormatter.ColorPositiveNegative<float>(),
		BonusAdmission<float>.Subtract,
		SupportedApplicationType.Percentage,
		StatProcessors.Percentage());

	public static readonly StatDefinition<float> HealthRegen = new("Health Regen",
		StatDisplayFormatter.PlainNameFormat<float>("#ff9771"),
		StatDisplayFormatter.PlainValueFormat<float>(),
		StatDisplayFormatter.ColorPositiveNegative<float>(),
		BonusAdmission<float>.AddAndSubtract,
		SupportedApplicationType.FlatAndPercentage,
		StatProcessors.Multiplicative<float>());

	public static readonly StatDefinition<float> ManaRegen = new("Mana Regen",
		StatDisplayFormatter.PlainNameFormat<float>("#71C9FF"),
		StatDisplayFormatter.PlainValueFormat<float>(),
		StatDisplayFormatter.ColorPositiveNegative<float>(),
		BonusAdmission<float>.AddAndSubtract,
		SupportedApplicationType.FlatAndPercentage,
		StatProcessors.Multiplicative<float>());

	public static readonly StatDefinition<int> PhysicalDamage = new("Physical Damage",
		StatDisplayFormatter.PlainNameFormat<int>("#FFB347"),
		StatDisplayFormatter.PlainValueFormat<int>(),
		StatDisplayFormatter.ColorPositiveNegative<int>(),
		BonusAdmission<int>.AddAndSubtract,
		SupportedApplicationType.FlatAndPercentage,
		StatProcessors.Multiplicative<int>());

	public static readonly StatDefinition<int> RitualDamage = new("Ritual Damage",
		StatDisplayFormatter.PlainNameFormat<int>("#DA6BFF"),
		StatDisplayFormatter.PlainValueFormat<int>(),
		StatDisplayFormatter.ColorPositiveNegative<int>(),
		BonusAdmission<int>.AddAndSubtract,
		SupportedApplicationType.FlatAndPercentage,
		StatProcessors.Multiplicative<int>());

	public static readonly StatDefinition<float> AttackSpeed = new("Attack Speed",
		StatDisplayFormatter.PlainNameFormat<float>("#FFE066"),
		StatDisplayFormatter.PercentageValueFormat(),
		StatDisplayFormatter.ColorPositiveNegative<float>(),
		BonusAdmission<float>.AddAndSubtract,
		SupportedApplicationType.Percentage,
		StatProcessors.Percentage());

	public static readonly StatDefinition<float> CritChance = new("Crit Chance",
		StatDisplayFormatter.PlainNameFormat<float>("#F4C430"),
		StatDisplayFormatter.PercentageValueFormat(),
		StatDisplayFormatter.ColorPositiveNegative<float>(),
		BonusAdmission<float>.None,
		SupportedApplicationType.Percentage,
		StatProcessors.Percentage());

	public static readonly StatDefinition<float> CritMultiplier = new("Crit Multiplier",
		StatDisplayFormatter.PlainNameFormat<float>("#FFBF00"),
		value => $"x{StatDisplayFormatter.PlainValueFormat<float>()(value)}",
		StatDisplayFormatter.ColorPositiveNegative<float>(),
		BonusAdmission<float>.None,
		SupportedApplicationType.Flat,
		StatProcessors.Flat<float>());

	public static readonly StatDefinition<float> Luck = new("Luck",
		StatDisplayFormatter.PlainNameFormat<float>("#77DD77"),
		StatDisplayFormatter.PercentageValueFormat(),
		StatDisplayFormatter.ColorPositiveNegative<float>(),
		BonusAdmission<float>.AddAndSubtract,
		SupportedApplicationType.Percentage,
		StatProcessors.Percentage());

	public static readonly StatDefinition<float> LootBonus = new("Loot Bonus",
		StatDisplayFormatter.PlainNameFormat<float>("#C2FF6B"),
		StatDisplayFormatter.PercentageValueFormat(),
		StatDisplayFormatter.ColorPositiveNegative<float>(),
		BonusAdmission<float>.AddAndSubtract,
		SupportedApplicationType.Percentage,
		StatProcessors.Percentage());


	public string baseName { get; }
	public Func<TValue, string, string> displayNameFormat { get; }
	public Func<TValue, string> valueFormat { get; }
	public BonusAdmission<TValue> bonusValueAdmission { get; }
	public Func<TValue, string>? valueColorSupplier { get; }
	public SupportedApplicationType supportedApplications { get; }
	public IStatProcessor<TValue> processor { get; }
	public Type valueType => typeof(TValue);

	private StatDefinition(string baseName, Func<TValue, string, string> displayNameFormat, Func<TValue, string> valueFormat,
					Func<TValue, string>? valueColorSupplier, BonusAdmission<TValue> bonusValueAdmission,	
					SupportedApplicationType supportedApplications, IStatProcessor<TValue> processor) {
		this.displayNameFormat = displayNameFormat;
		this.baseName = baseName;
		this.valueFormat = valueFormat;
		this.bonusValueAdmission = bonusValueAdmission;
		this.valueColorSupplier = valueColorSupplier;
		this.supportedApplications = supportedApplications;
		this.processor = processor;
	}

	string IStatDefinitionImpl.GetFormattedName(object value) => displayNameFormat.Invoke((TValue)value, baseName);

	string IStatDefinitionImpl.GetFormattedValue(object value, bool applyAsBonus) {
		string formattedValue = valueFormat.Invoke((TValue)value);
		if (applyAsBonus && valueColorSupplier != null) {
			formattedValue = string.Concat(bonusValueAdmission.GetPrefix((TValue)value), formattedValue);
			formattedValue = $"<color={valueColorSupplier((TValue)value)}>{formattedValue}</color>";
		} else {
			formattedValue = displayNameFormat.Invoke((TValue) value, formattedValue).Replace("s", "");						// hard-coded, not optimal
		}
		return formattedValue;
	}
}