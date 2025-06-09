using System;

public class StatType<TValue> : IStatTypeImpl {

	// TODO: implement BonusAdmissionSign (+[value] or -[value]), color coding
	public static readonly StatType<int> MaxHealth = new("Max Health", IntPlainNameFormat, PlainIntValueFormat);
	public static readonly StatType<int> MaxMana = new("Max Mana", IntPlainNameFormat, PlainIntValueFormat);
	public static readonly StatType<int> Defense = new("Defense", IntPlainNameFormat, PlainIntValueFormat);
	public static readonly StatType<int> SoulSlots = new("Soul Slot", IntPluralAdaptedNameFormat, PlainIntValueFormat);
	public static readonly StatType<float> MovementSpeed = new("Movement Speed", FloatPlainNameFormat, PercentageValueFormat);
	public static readonly StatType<int> JumpHeight = new("Jump Height", IntPlainNameFormat, PlainIntValueFormat);
	public static readonly StatType<int> MaxJumps = new("Max Jump", IntPluralAdaptedNameFormat, PlainIntValueFormat);
	public static readonly StatType<float> DashVelocity = new("Dash Velocity", FloatPlainNameFormat, PercentageValueFormat);
	public static readonly StatType<float> DashCooldown = new("Dash Cooldown", FloatPlainNameFormat, value => $"-{PercentageValueFormat(value)}");
	public static readonly StatType<float> HealthRegen = new("Health Regen", FloatPlainNameFormat, PlainFloatValueFormat);
	public static readonly StatType<float> ManaRegen = new("Mana Regen", FloatPlainNameFormat, PlainFloatValueFormat);
	public static readonly StatType<int> PhysicalDamage = new("Physical Damage", IntPlainNameFormat, PlainIntValueFormat);
	public static readonly StatType<int> RitualDamage = new("Ritual Damage", IntPlainNameFormat, PlainIntValueFormat);
	public static readonly StatType<float> AttackSpeed = new("Attack Speed", FloatPlainNameFormat, PercentageValueFormat);
	public static readonly StatType<float> CritChance = new("Crit Chance", FloatPlainNameFormat, PercentageValueFormat);
	public static readonly StatType<float> CritMultiplier = new("Crit Multiplier", FloatPlainNameFormat, value => $"x{PlainFloatValueFormat(value)}");
	public static readonly StatType<float> Luck = new("Luck", FloatPlainNameFormat, PercentageValueFormat);
	public static readonly StatType<float> LootBonus = new("LootBonus", FloatPlainNameFormat, PercentageValueFormat);

	public delegate string StatValueFormat(TValue value);

	public static StatType<float>.StatValueFormat PercentageValueFormat => value => $"{value * 100}%";
	public static StatType<float>.StatValueFormat PlainFloatValueFormat => value => value.ToString();
	public static StatType<int>.StatValueFormat PlainIntValueFormat => value => value.ToString();

	public static Func<int, string, string> IntPlainNameFormat => (value, baseName) => baseName;
	public static Func<float, string, string> FloatPlainNameFormat => (value, baseName) => baseName;
	public static Func<int, string, string> IntPluralAdaptedNameFormat => (value, baseName) => value > 1 ? $"{baseName}s" : baseName;
	public static Func<float, string, string> FloatPluralAdaptedNameFormat => (value, baseName) => value > 1 ? $"{baseName}s" : baseName;


	private readonly string baseName;
	public string BaseName => baseName;

	private readonly Func<TValue, string, string> displayNameFormat;
	public Func<TValue, string, string> DisplayNameFormat => displayNameFormat;

	private readonly StatValueFormat valueFormat;
	public StatValueFormat ValueFormat => valueFormat;

	public StatType(string baseName, Func<TValue, string, string> displayNameFormat, StatValueFormat valueFormat) {
		this.displayNameFormat = displayNameFormat;
		this.baseName = baseName;
		this.valueFormat = valueFormat;
	}

	public string GetFormattedName(TValue value) => displayNameFormat.Invoke(value, baseName);

	string IStatTypeImpl.GetFormattedName(object value) => GetFormattedName((TValue)value);
	string IStatTypeImpl.GetFormattedValue(object value) => valueFormat.Invoke((TValue)value);
}