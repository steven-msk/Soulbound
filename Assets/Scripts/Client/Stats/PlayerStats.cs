using System;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;
using Unity.VisualScripting;

public sealed class PlayerStats {
	private static readonly Logger logger = Logger.CreateInstance();
	private static readonly LogModule playerStats = new LogModule("PLAYER STATS", "#00FFFF");

	[JsonProperty]
	[JsonConverter(typeof(JsonDictionaryConverter<IStatDefinitionImpl, IStatEntryImpl>))]
	private static readonly Dictionary<IStatDefinitionImpl, IStatEntryImpl> injected = new();

	// REMINDER: current stat default values are subject to change
	[JsonIgnore] public StatEntry<int> MaxHealth { get; } = InjectStatEntry<int>(new(200, StatDefinition<int>.MaxHealth));
	[JsonIgnore] public StatEntry<int> MaxMana { get; } = InjectStatEntry<int>(new(50, StatDefinition<int>.MaxMana));
	[JsonIgnore] public StatEntry<int> Defense { get; } = InjectStatEntry<int>(new(0, StatDefinition<int>.Defense));
	[JsonIgnore] public StatEntry<int> SoulSlots { get; } = InjectStatEntry<int>(new(2, StatDefinition<int>.SoulSlots));
	[JsonIgnore] public StatEntry<float> MovementSpeed { get; } = InjectStatEntry<float>(new(8f, StatDefinition<float>.MovementSpeed));
	[JsonIgnore] public StatEntry<int> JumpHeight { get; } = InjectStatEntry<int>(new(1, StatDefinition<int>.JumpHeight));
	public int MaxJumps { get; set; } = 1;
	[JsonIgnore] public StatEntry<float> DashVelocity { get; } = InjectStatEntry<float>(new(1f, StatDefinition<float>.DashVelocity));
	[JsonIgnore] public StatEntry<float> DashCooldown { get; } = InjectStatEntry<float>(new(2f, StatDefinition<float>.DashCooldown));
	[JsonIgnore] public StatEntry<float> HealthRegen { get; } = InjectStatEntry<float>(new(1.5f, StatDefinition<float>.HealthRegen));
	[JsonIgnore] public StatEntry<float> ManaRegen { get; } = InjectStatEntry<float>(new(2f, StatDefinition<float>.ManaRegen));
	[JsonIgnore] public StatEntry<int> RawPhysicalDamage { get; } = InjectStatEntry<int>(new(10, StatDefinition<int>.PhysicalDamage));
	[JsonIgnore] public StatEntry<int> RawRitualDamage { get; } = InjectStatEntry<int>(new(10, StatDefinition<int>.RitualDamage));
	[JsonIgnore] public StatEntry<float> AttackSpeed { get; } = InjectStatEntry<float>(new(1f, StatDefinition<float>.AttackSpeed));
	[JsonIgnore] public StatEntry<float> CritChance { get; } = InjectStatEntry<float>(new(0.05f, StatDefinition<float>.CritChance));
	[JsonIgnore] public StatEntry<float> CritMultiplier { get; } = InjectStatEntry<float>(new(1.5f, StatDefinition<float>.CritMultiplier));
	[JsonIgnore] public StatEntry<float> Luck { get; } = InjectStatEntry<float>(new(0f, StatDefinition<float>.Luck));
	[JsonIgnore] public StatEntry<float> LootBonus { get; } = InjectStatEntry<float>(new(0f, StatDefinition<float>.LootBonus));
	public float HorizontalAcceleration { get; set; } = 1f;
	public float HorizontalFlightAcceleration { get; set; } = 3f;
	public float VerticalFlightAcceleration { get; set; } = 2f;
	public int GrantedFlightTime { get; set; } = 0;

	internal static StatEntry<TValue> InjectStatEntry<TValue>(StatEntry<TValue> statEntry) where TValue : struct, IComparable<TValue> {
		injected[statEntry.definitionReference] = statEntry;
		return statEntry;
	}

	public void Apply(List<AbstractSerializableStat> stats, IStatProvider source) {
		Invoke(stats, source, (statEntry, serializableStat, source) => statEntry.Add(serializableStat, source));
	}

	public void Apply(AbstractSerializableStat stat, IStatProvider source) => Apply(new List<AbstractSerializableStat>() { stat }, source);

	public void Revoke(List<AbstractSerializableStat> stats, IStatProvider source) {
		Invoke(stats, source, (statEntry, serializableStat, source) => statEntry.Remove(serializableStat, source));
	}

	public void Revoke(AbstractSerializableStat stat, IStatProvider source) => Revoke(new List<AbstractSerializableStat> { stat }, source);

	
	private void Invoke(List<AbstractSerializableStat> stats, IStatProvider source, Action<IStatEntryImpl, AbstractSerializableStat, IStatProvider> statAction) {
		stats.ForEach(stat => {
			if (injected.TryGetValue(stat.GetStatDefinition(), out var statDefinition)) {
				statAction.Invoke(statDefinition, stat, source);
			} else {
				throw new InvalidStatDefinitionBindingException(stat);
			}
		});
	}

	//private float healthRegenAccumulator = 0;
	//private float manaRegenAccumulator = 0;


	//public void UpdateRegenStats() {
	//	healthRegenAccumulator += healthRegen * Time.deltaTime;
	//	if (healthRegenAccumulator >= 1f) {
	//		int regenAmount = Mathf.FloorToInt(healthRegenAccumulator);
	//		Health += regenAmount;
	//		healthRegenAccumulator -= regenAmount;
	//	}

	//	manaRegenAccumulator += manaRegen * Time.deltaTime;
	//	if (manaRegenAccumulator >= 1f) {
	//		int regenAmount = Mathf.FloorToInt(manaRegenAccumulator);
	//		Mana += regenAmount;
	//		manaRegenAccumulator -= regenAmount;
	//	}
	//}

	private class InvalidStatDefinitionBindingException : NullReferenceException {
		public InvalidStatDefinitionBindingException(AbstractSerializableStat stat)
			: base ($"SerializableStat {stat.GetStatDefinition()} does not contain a valid stat binding. Could not find stat definition {stat.GetStatDefinition()}") { }
	}
}


