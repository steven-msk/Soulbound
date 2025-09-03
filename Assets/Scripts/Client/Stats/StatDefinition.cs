using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Plastic.Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

#nullable enable

public class StatDefinition<TValue> : IStatDefinitionImpl where TValue : struct, IComparable<TValue> {
	public static readonly StatDefinition<int> MaxHealth = InjectID("maxHealth", new StatDefinition<int>("Max Health",
		StatDisplayFormatter.PlainNameFormat<int>("#FF6B6B"),
		StatDisplayFormatter.PlainValueFormat<int>(),
		StatDisplayFormatter.ColorPositiveNegative<int>(),
		BonusAdmission<int>.AddAndSubtract,
		SupportedApplicationType.Flat,
		StatProcessors.Flat<int>()
	));
	public static readonly StatDefinition<int> MaxMana = InjectID("maxMana", new StatDefinition<int>("Max Mana",
		StatDisplayFormatter.PlainNameFormat<int>("#6BCBFF"),
		StatDisplayFormatter.PlainValueFormat<int>(),
		StatDisplayFormatter.ColorPositiveNegative<int>(),
		BonusAdmission<int>.AddAndSubtract,
		SupportedApplicationType.Flat,
		StatProcessors.Flat<int>()
	));
	public static readonly StatDefinition<int> Defense = InjectID("defense", new StatDefinition<int>("Defense",
		StatDisplayFormatter.PlainNameFormat<int>("#CCDDEE"),
		StatDisplayFormatter.PlainValueFormat<int>(),
		StatDisplayFormatter.ColorPositiveNegative<int>(),
		BonusAdmission<int>.AddAndSubtract,
		SupportedApplicationType.Flat,
		StatProcessors.Flat<int>()
	));
	public static readonly StatDefinition<int> SoulSlots = InjectID("soulSlot", new StatDefinition<int>("Soul Slot",
		StatDisplayFormatter.PluralAdaptedNameFormat<int>("#C86BFF"),
		StatDisplayFormatter.PlainValueFormat<int>(),
		StatDisplayFormatter.ColorPositiveNegative<int>(),
		BonusAdmission<int>.AddAndSubtract,
		SupportedApplicationType.Flat,
		StatProcessors.Flat<int>()
	));
	public static readonly StatDefinition<float> MovementSpeed = InjectID("movementSpeed", new StatDefinition<float>("Movement Speed",
		StatDisplayFormatter.PlainNameFormat<float>("#6BFFB6"),
		StatDisplayFormatter.PercentageValueFormat(),
		StatDisplayFormatter.ColorPositiveNegative<float>(),
		BonusAdmission<float>.AddAndSubtract,
		SupportedApplicationType.Percentage,
		StatProcessors.Percentage()
	));
	public static readonly StatDefinition<int> JumpHeight = InjectID("jumpHeight", new StatDefinition<int>("Jump Height",
		StatDisplayFormatter.PlainNameFormat<int>("#67E8F9"),
		StatDisplayFormatter.PlainValueFormat<int>(),
		StatDisplayFormatter.ColorPositiveNegative<int>(),
		BonusAdmission<int>.Add,
		SupportedApplicationType.Flat,
		StatProcessors.Flat<int>()
	));
	public static readonly StatDefinition<int> MaxJumps = InjectID("maxJump", new StatDefinition<int>("Max Jump",
		StatDisplayFormatter.PluralAdaptedNameFormat<int>("#67E8F9"),
		StatDisplayFormatter.PlainValueFormat<int>(),
		StatDisplayFormatter.ColorPositiveNegative<int>(),
		BonusAdmission<int>.Add,
		SupportedApplicationType.Flat,
		StatProcessors.Flat<int>()
	));
	public static readonly StatDefinition<float> DashVelocity = InjectID("dashVelocity", new StatDefinition<float>("Dash Velocity",
		StatDisplayFormatter.PlainNameFormat<float>("#5EEAD4"),
		StatDisplayFormatter.PercentageValueFormat(),
		StatDisplayFormatter.ColorPositiveNegative<float>(),
		BonusAdmission<float>.AddAndSubtract,
		SupportedApplicationType.Percentage,
		StatProcessors.Percentage()
	));
	public static readonly StatDefinition<float> DashCooldown = InjectID("dashCooldown", new StatDefinition<float>("Dash Cooldown",
		StatDisplayFormatter.PlainNameFormat<float>("#4BFFE0"),
		value => $"-{StatDisplayFormatter.PercentageValueFormat()(value)}",
		StatDisplayFormatter.ColorPositiveNegative<float>(),
		BonusAdmission<float>.Subtract,
		SupportedApplicationType.Percentage,
		StatProcessors.Percentage()
	));
	public static readonly StatDefinition<float> HealthRegen = InjectID("healthRegen", new StatDefinition<float>("Health Regen",
		StatDisplayFormatter.PlainNameFormat<float>("#ff9771"),
		StatDisplayFormatter.PlainValueFormat<float>(),
		StatDisplayFormatter.ColorPositiveNegative<float>(),
		BonusAdmission<float>.AddAndSubtract,
		SupportedApplicationType.FlatAndPercentage,
		StatProcessors.Multiplicative<float>()
	));
	public static readonly StatDefinition<float> ManaRegen = InjectID("manaRegen", new StatDefinition<float>("Mana Regen",
		StatDisplayFormatter.PlainNameFormat<float>("#71C9FF"),
		StatDisplayFormatter.PlainValueFormat<float>(),
		StatDisplayFormatter.ColorPositiveNegative<float>(),
		BonusAdmission<float>.AddAndSubtract,
		SupportedApplicationType.FlatAndPercentage,
		StatProcessors.Multiplicative<float>()
	));
	public static readonly StatDefinition<int> PhysicalDamage = InjectID("physicalDamage", new StatDefinition<int>("Physical Damage",
		StatDisplayFormatter.PlainNameFormat<int>("#FFB347"),
		StatDisplayFormatter.PlainValueFormat<int>(),
		StatDisplayFormatter.ColorPositiveNegative<int>(),
		BonusAdmission<int>.AddAndSubtract,
		SupportedApplicationType.FlatAndPercentage,
		StatProcessors.Multiplicative<int>()
	));
	public static readonly StatDefinition<int> RitualDamage = InjectID("ritualDamage", new StatDefinition<int>("Ritual Damage",
		StatDisplayFormatter.PlainNameFormat<int>("#DA6BFF"),
		StatDisplayFormatter.PlainValueFormat<int>(),
		StatDisplayFormatter.ColorPositiveNegative<int>(),
		BonusAdmission<int>.AddAndSubtract,
		SupportedApplicationType.FlatAndPercentage,
		StatProcessors.Multiplicative<int>()
	));
	public static readonly StatDefinition<float> AttackSpeed = InjectID("attackSpeed", new StatDefinition<float>("Attack Speed",
		StatDisplayFormatter.PlainNameFormat<float>("#FFE066"),
		StatDisplayFormatter.PercentageValueFormat(),
		StatDisplayFormatter.ColorPositiveNegative<float>(),
		BonusAdmission<float>.AddAndSubtract,
		SupportedApplicationType.Percentage,
		StatProcessors.Percentage()
	));
	public static readonly StatDefinition<float> CritChance = InjectID("critChance", new StatDefinition<float>("Crit Chance",
		StatDisplayFormatter.PlainNameFormat<float>("#F4C430"),
		StatDisplayFormatter.PercentageValueFormat(),
		StatDisplayFormatter.ColorPositiveNegative<float>(),
		BonusAdmission<float>.None,
		SupportedApplicationType.Percentage,
		StatProcessors.Percentage()
	));
	public static readonly StatDefinition<float> CritMultiplier = InjectID("critMultiplier", new StatDefinition<float>("Crit Multiplier",
		StatDisplayFormatter.PlainNameFormat<float>("#FFBF00"),
		value => $"x{StatDisplayFormatter.PlainValueFormat<float>()(value)}",
		StatDisplayFormatter.ColorPositiveNegative<float>(),
		BonusAdmission<float>.None,
		SupportedApplicationType.Flat,
		StatProcessors.Flat<float>()
	));
	public static readonly StatDefinition<float> Luck = InjectID("luck", new StatDefinition<float>("Luck",
		StatDisplayFormatter.PlainNameFormat<float>("#77DD77"),
		StatDisplayFormatter.PercentageValueFormat(),
		StatDisplayFormatter.ColorPositiveNegative<float>(),
		BonusAdmission<float>.AddAndSubtract,
		SupportedApplicationType.Percentage,
		StatProcessors.Percentage()
	));
	public static readonly StatDefinition<float> LootBonus = InjectID("lootBonus", new StatDefinition<float>("Loot Bonus",
		StatDisplayFormatter.PlainNameFormat<float>("#C2FF6B"),
		StatDisplayFormatter.PercentageValueFormat(),
		StatDisplayFormatter.ColorPositiveNegative<float>(),
		BonusAdmission<float>.AddAndSubtract,
		SupportedApplicationType.Percentage,
		StatProcessors.Percentage()
	));


	public string baseName { get; }
	public Func<TValue, string, string> displayNameFormat { get; }
	public Func<TValue, string> valueFormat { get; }
	public BonusAdmission<TValue> bonusValueAdmission { get; }
	public Func<TValue, string>? valueColorSupplier { get; }
	public SupportedApplicationType supportedApplications { get; }
	public IStatProcessor<TValue> processor { get; }
	public string? id { get; set; }
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
			formattedValue = displayNameFormat.Invoke((TValue)value, formattedValue).Replace("s", "");                      // hard-coded, not optimal
		}
		return formattedValue;
	}

	protected static StatDefinition<TInnerValue> InjectID<TInnerValue>(string id, StatDefinition<TInnerValue> definition)
			where TInnerValue : struct, IComparable<TInnerValue> {
		definition.id = id;
		IStatDefinitionImpl.Register(id, definition);
		return definition;
	}

	public override string ToString() {
		return $"StatDefinition<{typeof(TValue)}>[type: {valueType}, baseName: {baseName}]";
	}

	// POTENTIAL FIXME: there could be some memory leaks regarding StatDefinition<> instances - some types get instantiated through serialization while other through static definition

	public override int GetHashCode() {
		return this.id!.GetHashCode();
	}

	public override bool Equals(object obj) {
		return obj is IStatDefinitionImpl other && other.id == this.id;
	}

	public static bool operator ==(StatDefinition<TValue> first, StatDefinition<TValue> second) {
		return first.id == second.id;
	}

	public static bool operator !=(StatDefinition<TValue> first, StatDefinition<TValue> second) {
		return !(first == second);
	}
}