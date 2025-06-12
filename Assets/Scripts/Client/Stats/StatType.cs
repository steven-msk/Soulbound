using System;

public class StatType<TValue> : IStatTypeImpl {
	public static readonly StatType<int> MaxHealth = new("Max Health", StatDisplayFormatter.PlainNameFormat<int>(), StatDisplayFormatter.PlainValueFormat<int>(), 
		BonusAdmission<int>.AddAndSubtract, StatDisplayFormatter.ColorPositiveNegative<int>());
	
	public static readonly StatType<int> MaxMana = new("Max Mana", StatDisplayFormatter.PlainNameFormat<int>(), StatDisplayFormatter.PlainValueFormat<int>(), 
		BonusAdmission<int>.AddAndSubtract, StatDisplayFormatter.ColorPositiveNegative<int>());
	
	public static readonly StatType<int> Defense = new("Defense", StatDisplayFormatter.PlainNameFormat<int>(), StatDisplayFormatter.PlainValueFormat<int>(), 
		BonusAdmission<int>.AddAndSubtract, StatDisplayFormatter.ColorPositiveNegative<int>());
	
	public static readonly StatType<int> SoulSlots = new("Soul Slot", StatDisplayFormatter.PluralAdaptedNameFormat<int>(), StatDisplayFormatter.PlainValueFormat<int>(), 
		BonusAdmission<int>.AddAndSubtract, StatDisplayFormatter.ColorPositiveNegative<int>());
	
	public static readonly StatType<float> MovementSpeed = new("Movement Speed", StatDisplayFormatter.PlainNameFormat<float>(), StatDisplayFormatter.PercentageValueFormat(), 
		BonusAdmission<float>.AddAndSubtract, StatDisplayFormatter.ColorPositiveNegative<float>());
	
	public static readonly StatType<int> JumpHeight = new("Jump Height", StatDisplayFormatter.PlainNameFormat<int>(), StatDisplayFormatter.PlainValueFormat<int>(), 
		BonusAdmission<int>.Add, StatDisplayFormatter.ColorPositiveNegative<int>());
	
	public static readonly StatType<int> MaxJumps = new("Max Jump", StatDisplayFormatter.PluralAdaptedNameFormat<int>(), StatDisplayFormatter.PlainValueFormat<int>(),
		BonusAdmission<int>.Add, StatDisplayFormatter.ColorPositiveNegative<int>());
	
	public static readonly StatType<float> DashVelocity = new("Dash Velocity", StatDisplayFormatter.PlainNameFormat<float>(), StatDisplayFormatter.PercentageValueFormat(),
		BonusAdmission<float>.AddAndSubtract, StatDisplayFormatter.ColorPositiveNegative<float>());
	
	public static readonly StatType<float> DashCooldown = new("Dash Cooldown", StatDisplayFormatter.PlainNameFormat<float>(), 
		value => $"-{StatDisplayFormatter.PercentageValueFormat()(value)}", BonusAdmission<float>.Subtract, _ => "green");
	
	public static readonly StatType<float> HealthRegen = new("Health Regen", StatDisplayFormatter.PlainNameFormat<float>(), StatDisplayFormatter.PlainValueFormat<float>(),
		BonusAdmission<float>.AddAndSubtract, StatDisplayFormatter.ColorPositiveNegative<float>());
	
	public static readonly StatType<float> ManaRegen = new("Mana Regen", StatDisplayFormatter.PlainNameFormat<float>(), StatDisplayFormatter.PlainValueFormat<float>(),
		BonusAdmission<float>.AddAndSubtract, StatDisplayFormatter.ColorPositiveNegative<float>());
	
	public static readonly StatType<int> PhysicalDamage = new("Physical Damage", StatDisplayFormatter.PlainNameFormat<int>(), StatDisplayFormatter.PlainValueFormat<int>(),
		BonusAdmission<int>.AddAndSubtract, StatDisplayFormatter.ColorPositiveNegative<int>());
	
	public static readonly StatType<int> RitualDamage = new("Ritual Damage", StatDisplayFormatter.PlainNameFormat<int>(), StatDisplayFormatter.PlainValueFormat<int>(),
		BonusAdmission<int>.AddAndSubtract, StatDisplayFormatter.ColorPositiveNegative<int>());
	
	public static readonly StatType<float> AttackSpeed = new("Attack Speed", StatDisplayFormatter.PlainNameFormat<float>(), StatDisplayFormatter.PercentageValueFormat(),
		BonusAdmission<float>.AddAndSubtract, StatDisplayFormatter.ColorPositiveNegative<float>());
	
	public static readonly StatType<float> CritChance = new("Crit Chance", StatDisplayFormatter.PlainNameFormat<float>(), StatDisplayFormatter.PercentageValueFormat(),
		BonusAdmission<float>.None);
	
	public static readonly StatType<float> CritMultiplier = new("Crit Multiplier", StatDisplayFormatter.PlainNameFormat<float>(), 
		value => $"x{StatDisplayFormatter.PlainValueFormat<float>()(value)}", BonusAdmission<float>.None);
	
	public static readonly StatType<float> Luck = new("Luck", StatDisplayFormatter.PlainNameFormat<float>(), StatDisplayFormatter.PercentageValueFormat(),
		BonusAdmission<float>.AddAndSubtract, StatDisplayFormatter.ColorPositiveNegative<float>());
	
	public static readonly StatType<float> LootBonus = new("Loot Bonus", StatDisplayFormatter.PlainNameFormat<float>(), StatDisplayFormatter.PercentageValueFormat(), 
		BonusAdmission<float>.AddAndSubtract, StatDisplayFormatter.ColorPositiveNegative<float>());


	private readonly string baseName;
	public string BaseName => baseName;

	private readonly Func<TValue, string, string> displayNameFormat;
	public Func<TValue, string, string> DisplayNameFormat => displayNameFormat;

	private readonly Func<TValue, string> valueFormat;
	public Func<TValue, string> ValueFormat => valueFormat;

	private readonly BonusAdmission<TValue> bonusValueAdmission;
	public BonusAdmission<TValue> BonusValueAdmission => bonusValueAdmission;

	private readonly Func<TValue, string> colorSupplier;
	public Func<TValue, string> ColorSupplier => colorSupplier;

	private StatType(string baseName, Func<TValue, string, string> displayNameFormat, Func<TValue, string> valueFormat, 
					 BonusAdmission<TValue> bonusValueAdmission, Func<TValue, string> colorSupplier = null) {
		this.displayNameFormat = displayNameFormat;
		this.baseName = baseName;
		this.valueFormat = valueFormat;
		this.bonusValueAdmission = bonusValueAdmission;
		this.colorSupplier = colorSupplier ?? (_ => "white");
	}



	string IStatTypeImpl.GetFormattedName(object value) => displayNameFormat.Invoke((TValue)value, baseName);

	string IStatTypeImpl.GetFormattedValue(object value, bool applyAsBonus) {
		string formattedValue = valueFormat.Invoke((TValue)value);
		if (applyAsBonus) {
			formattedValue = $"<color={colorSupplier((TValue)value)}>{string.Concat(bonusValueAdmission.GetPrefix((TValue)value), formattedValue)}</color>";
		}
		return formattedValue;
	}
}