using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class StatType<TValue> : IStatTypeImpl {
	public static readonly StatType<int> MaxHealth = new("Max Health", 
		StatDisplayFormatter.PlainNameFormat<int>("#FF6B6B"),
		StatDisplayFormatter.PlainValueFormat<int>(), 
		BonusAdmission<int>.AddAndSubtract,
		new HashSet<SerializableStat.StatApplicationType>() {
			SerializableStat.StatApplicationType.Flat
		},
		StatDisplayFormatter.ColorPositiveNegative<int>());
	
	public static readonly StatType<int> MaxMana = new("Max Mana",
		StatDisplayFormatter.PlainNameFormat<int>("#6BCBFF"),
		StatDisplayFormatter.PlainValueFormat<int>(), 
		BonusAdmission<int>.AddAndSubtract, 
		new HashSet<SerializableStat.StatApplicationType>() {
			SerializableStat.StatApplicationType.Flat
		},
		StatDisplayFormatter.ColorPositiveNegative<int>());
	
	public static readonly StatType<int> Defense = new("Defense",
		StatDisplayFormatter.PlainNameFormat<int>("#CCDDEE"),
		StatDisplayFormatter.PlainValueFormat<int>(), 
		BonusAdmission<int>.AddAndSubtract,
		new HashSet<SerializableStat.StatApplicationType>() {
			SerializableStat.StatApplicationType.Flat
		},
		StatDisplayFormatter.ColorPositiveNegative<int>());
	
	public static readonly StatType<int> SoulSlots = new("Soul Slot", 
		StatDisplayFormatter.PluralAdaptedNameFormat<int>("#c86bff"), 
		StatDisplayFormatter.PlainValueFormat<int>(), 
		BonusAdmission<int>.AddAndSubtract,
		new HashSet<SerializableStat.StatApplicationType>() {
			SerializableStat.StatApplicationType.Flat
		},
		StatDisplayFormatter.ColorPositiveNegative<int>()); 
	
	public static readonly StatType<float> MovementSpeed = new("Movement Speed", 
		StatDisplayFormatter.PlainNameFormat<float>("#6BFFB6"),
		StatDisplayFormatter.PercentageValueFormat(), 
		BonusAdmission<float>.AddAndSubtract,
		new HashSet<SerializableStat.StatApplicationType>() {
			SerializableStat.StatApplicationType.Percentage
		},
		StatDisplayFormatter.ColorPositiveNegative<float>());
	
	public static readonly StatType<int> JumpHeight = new("Jump Height", 
		StatDisplayFormatter.PlainNameFormat<int>("#67E8F9"),
		StatDisplayFormatter.PlainValueFormat<int>(), 
		BonusAdmission<int>.Add,
		new HashSet<SerializableStat.StatApplicationType>() {
			SerializableStat.StatApplicationType.Flat
		},
		StatDisplayFormatter.ColorPositiveNegative<int>());
	
	public static readonly StatType<int> MaxJumps = new("Max Jump", 
		StatDisplayFormatter.PluralAdaptedNameFormat<int>("#67E8F9"), 
		StatDisplayFormatter.PlainValueFormat<int>(),
		BonusAdmission<int>.Add,
		new HashSet<SerializableStat.StatApplicationType>() {
			SerializableStat.StatApplicationType.Flat
		},
		StatDisplayFormatter.ColorPositiveNegative<int>());
	
	public static readonly StatType<float> DashVelocity = new("Dash Velocity",
		StatDisplayFormatter.PlainNameFormat<float>("#5EEAD4"), 
		StatDisplayFormatter.PercentageValueFormat(),
		BonusAdmission<float>.AddAndSubtract,
		new HashSet<SerializableStat.StatApplicationType>() {
			SerializableStat.StatApplicationType.Percentage
		},
		StatDisplayFormatter.ColorPositiveNegative<float>());

	public static readonly StatType<float> DashCooldown = new("Dash Cooldown",
		StatDisplayFormatter.PlainNameFormat<float>("#4BFFE0"),
		value => $"-{StatDisplayFormatter.PercentageValueFormat()(value)}",
		BonusAdmission<float>.Subtract,
		new HashSet<SerializableStat.StatApplicationType>() {
			SerializableStat.StatApplicationType.Percentage
		},
		StatDisplayFormatter.ColorPositiveNegative<float>());

	public static readonly StatType<float> HealthRegen = new("Health Regen",
		StatDisplayFormatter.PlainNameFormat<float>("#ff9771"),
		StatDisplayFormatter.PlainValueFormat<float>(),
		BonusAdmission<float>.AddAndSubtract, 
		new HashSet<SerializableStat.StatApplicationType>() { 
			SerializableStat.StatApplicationType.Flat,
			SerializableStat.StatApplicationType.Percentage
		}, 
		StatDisplayFormatter.ColorPositiveNegative<float>());
	
	public static readonly StatType<float> ManaRegen = new("Mana Regen", 
		StatDisplayFormatter.PlainNameFormat<float>("#71C9FF"), 
		StatDisplayFormatter.PlainValueFormat<float>(),
		BonusAdmission<float>.AddAndSubtract,
		new HashSet<SerializableStat.StatApplicationType>() {
			SerializableStat.StatApplicationType.Flat,
			SerializableStat.StatApplicationType.Percentage
		},
		StatDisplayFormatter.ColorPositiveNegative<float>());
	
	public static readonly StatType<int> PhysicalDamage = new("Physical Damage",
		StatDisplayFormatter.PlainNameFormat<int>("#FFB347"),
		StatDisplayFormatter.PlainValueFormat<int>(),
		BonusAdmission<int>.AddAndSubtract, 
		new HashSet<SerializableStat.StatApplicationType>() {
			SerializableStat.StatApplicationType.Flat,
			SerializableStat.StatApplicationType.Percentage
		},
		StatDisplayFormatter.ColorPositiveNegative<int>());
	
	public static readonly StatType<int> RitualDamage = new("Ritual Damage",
		StatDisplayFormatter.PlainNameFormat<int>("#DA6BFF"),
		StatDisplayFormatter.PlainValueFormat<int>(),
		BonusAdmission<int>.AddAndSubtract,
		new HashSet<SerializableStat.StatApplicationType>() {
			SerializableStat.StatApplicationType.Flat,
			SerializableStat.StatApplicationType.Percentage
		},
		StatDisplayFormatter.ColorPositiveNegative<int>());

	public static readonly StatType<float> AttackSpeed = new("Attack Speed",
		StatDisplayFormatter.PlainNameFormat<float>("#FFE066"),
		StatDisplayFormatter.PercentageValueFormat(),
		BonusAdmission<float>.AddAndSubtract,
		new HashSet<SerializableStat.StatApplicationType>() {
			SerializableStat.StatApplicationType.Percentage
		},
		StatDisplayFormatter.ColorPositiveNegative<float>());
	
	public static readonly StatType<float> CritChance = new("Crit Chance",
		StatDisplayFormatter.PlainNameFormat<float>("#F4C430"), 
		StatDisplayFormatter.PercentageValueFormat(),
		BonusAdmission<float>.None,
		new HashSet<SerializableStat.StatApplicationType>() {
			SerializableStat.StatApplicationType.Percentage
		},
		StatDisplayFormatter.ColorPositiveNegative<float>());
	
	public static readonly StatType<float> CritMultiplier = new("Crit Multiplier", 
		StatDisplayFormatter.PlainNameFormat<float>("#FFBF00"),
		value => $"x{StatDisplayFormatter.PlainValueFormat<float>()(value)}",
		BonusAdmission<float>.None,
		new HashSet<SerializableStat.StatApplicationType>() {
			SerializableStat.StatApplicationType.Flat
		},
		StatDisplayFormatter.ColorPositiveNegative<float>());
	
	public static readonly StatType<float> Luck = new("Luck",
		StatDisplayFormatter.PlainNameFormat<float>("#77DD77"),
		StatDisplayFormatter.PercentageValueFormat(),
		BonusAdmission<float>.AddAndSubtract,
		new HashSet<SerializableStat.StatApplicationType>() {
			SerializableStat.StatApplicationType.Percentage
		},
		StatDisplayFormatter.ColorPositiveNegative<float>());
	
	public static readonly StatType<float> LootBonus = new("Loot Bonus", 
		StatDisplayFormatter.PlainNameFormat<float>("#C2FF6B"),
		StatDisplayFormatter.PercentageValueFormat(), 
		BonusAdmission<float>.AddAndSubtract,
		new HashSet<SerializableStat.StatApplicationType>() {
			SerializableStat.StatApplicationType.Percentage
		},
		StatDisplayFormatter.ColorPositiveNegative<float>());


	private readonly string baseName;
	public string BaseName => baseName;

	private readonly Func<TValue, string, string> displayNameFormat;
	public Func<TValue, string, string> DisplayNameFormat => displayNameFormat;

	private readonly Func<TValue, string> valueFormat;
	public Func<TValue, string> ValueFormat => valueFormat;

	private readonly BonusAdmission<TValue> bonusValueAdmission;
	public BonusAdmission<TValue> BonusValueAdmission => bonusValueAdmission;

	private readonly Func<TValue, string> valueColorSupplier;
	public Func<TValue, string> ColorSupplier => valueColorSupplier;

	private readonly HashSet<SerializableStat.StatApplicationType> validApplications;
	public HashSet<SerializableStat.StatApplicationType> ValidApplications => validApplications;

	public Type ValueType => typeof(TValue);

	private StatType(string baseName, Func<TValue, string, string> displayNameFormat, Func<TValue, string> valueFormat,
					BonusAdmission<TValue> bonusValueAdmission, HashSet<SerializableStat.StatApplicationType> validApplications,
					Func<TValue, string> valueColorSupplier = null) {
		this.displayNameFormat = displayNameFormat;
		this.baseName = baseName;
		this.valueFormat = valueFormat;
		this.bonusValueAdmission = bonusValueAdmission;
		this.valueColorSupplier = valueColorSupplier;
		this.validApplications = validApplications.ToHashSet();	
	}

	string IStatTypeImpl.GetFormattedName(object value) => displayNameFormat.Invoke((TValue)value, baseName);

	string IStatTypeImpl.GetFormattedValue(object value, bool applyAsBonus) {
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