using System;

#nullable enable

namespace SoulboundBackend.Client.Stats {
	public partial class StatDefinition<TValue> : IStatDefinition where TValue : struct, IComparable<TValue> {
		public string baseName { get; }

		public SupportedApplicationType supportedApplications { get; }
		public string? id { get; set; }
		public Type valueType => typeof(TValue);

		public StatDefinition(string baseName, SupportedApplicationType supportedApplications) {
			this.baseName = baseName;
			this.supportedApplications = supportedApplications;
		}

		public override string ToString() {
			return $"StatDefinition<{typeof(TValue)}>[type: {valueType}, baseName: {baseName}]";
		}
	}

	public partial class Stats {
		public static readonly StatDefinition<int> maxHealth = InjectID("maxHealth", new StatDefinition<int>("Max Health", SupportedApplicationType.FlatOnly));
		public static readonly StatDefinition<int> maxMana = InjectID("maxMana", new StatDefinition<int>("Max Mana", SupportedApplicationType.FlatOnly));
		public static readonly StatDefinition<int> defense = InjectID("defense", new StatDefinition<int>("Defense", SupportedApplicationType.FlatOnly));
		public static readonly StatDefinition<int> soulSlots = InjectID("soulSlot", new StatDefinition<int>("Soul Slot", SupportedApplicationType.FlatOnly));
		public static readonly StatDefinition<float> movementSpeed = InjectID("movementSpeed", new StatDefinition<float>("Movement Speed", SupportedApplicationType.PercentageOnly));
		public static readonly StatDefinition<int> jumpHeight = InjectID("jumpHeight", new StatDefinition<int>("Jump Height", SupportedApplicationType.FlatOnly));
		public static readonly StatDefinition<int> maxJumps = InjectID("maxJump", new StatDefinition<int>("Max Jump", SupportedApplicationType.FlatOnly));
		public static readonly StatDefinition<float> dashVelocity = InjectID("dashVelocity", new StatDefinition<float>("Dash Velocity", SupportedApplicationType.PercentageOnly));
		public static readonly StatDefinition<float> dashCooldown = InjectID("dashCooldown", new StatDefinition<float>("Dash Cooldown",	SupportedApplicationType.PercentageOnly));
		public static readonly StatDefinition<float> healthRegen = InjectID("healthRegen", new StatDefinition<float>("Health Regen", SupportedApplicationType.FlatAndPercentage));
		public static readonly StatDefinition<float> manaRegen = InjectID("manaRegen", new StatDefinition<float>("Mana Regen", SupportedApplicationType.FlatAndPercentage));
		public static readonly StatDefinition<int> physicalDamage = InjectID("physicalDamage", new StatDefinition<int>("Physical Damage", SupportedApplicationType.FlatAndPercentage));
		public static readonly StatDefinition<int> ritualDamage = InjectID("ritualDamage", new StatDefinition<int>("Ritual Damage",	SupportedApplicationType.FlatAndPercentage));
		public static readonly StatDefinition<float> attackSpeed = InjectID("attackSpeed", new StatDefinition<float>("Attack Speed", SupportedApplicationType.PercentageOnly		));
		public static readonly StatDefinition<float> critChance = InjectID("critChance", new StatDefinition<float>("Crit Chance", SupportedApplicationType.PercentageOnly));
		public static readonly StatDefinition<float> critMultiplier = InjectID("critMultiplier", new StatDefinition<float>("Crit Multiplier", SupportedApplicationType.FlatOnly));
		public static readonly StatDefinition<float> luck = InjectID("luck", new StatDefinition<float>("Luck", SupportedApplicationType.PercentageOnly));
		public static readonly StatDefinition<float> lootBonus = InjectID("lootBonus", new StatDefinition<float>("Loot Bonus", SupportedApplicationType.PercentageOnly));

		protected static StatDefinition<T> InjectID<T>(string id, StatDefinition<T> definition) where T : struct, IComparable<T> {
			definition.id = id;
			return definition;
		}
	}
}
