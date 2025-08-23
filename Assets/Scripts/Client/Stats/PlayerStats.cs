using System;
using System.Collections.Generic;

public class PlayerStats {
	private static readonly Dictionary<IStatDefinitionImpl, IStatEntryImpl> registeredStats = new();

	// REMINDER: current stat default values are subject to change
	public StatEntry<int> MaxHealth { get; } = InjectStatEntry<int>(new(200, StatDefinition<int>.MaxHealth));
	public StatEntry<int> MaxMana { get; } = InjectStatEntry<int>(new(50, StatDefinition<int>.MaxMana));
	public StatEntry<int> Defense { get; } = InjectStatEntry<int>(new(0, StatDefinition<int>.Defense));
	public StatEntry<int> SoulSlots { get; } = InjectStatEntry<int>(new(2, StatDefinition<int>.SoulSlots));
	public StatEntry<float> MovementSpeed { get; } = InjectStatEntry<float>(new(8f, StatDefinition<float>.MovementSpeed));
	public StatEntry<int> JumpHeight { get; } = InjectStatEntry<int>(new(1, StatDefinition<int>.JumpHeight));
	public int MaxJumps { get; set; } = 1;
	public StatEntry<float> DashVelocity { get; } = InjectStatEntry<float>(new(1f, StatDefinition<float>.DashVelocity));
	public StatEntry<float> DashCooldown { get; } = InjectStatEntry<float>(new(2f, StatDefinition<float>.DashCooldown));
	public StatEntry<float> HealthRegen { get; } = InjectStatEntry<float>(new(1.5f, StatDefinition<float>.HealthRegen));
	public StatEntry<float> ManaRegen { get; } = InjectStatEntry<float>(new(2f, StatDefinition<float>.ManaRegen));
	public StatEntry<int> RawPhysicalDamage { get; } = InjectStatEntry<int>(new(10, StatDefinition<int>.PhysicalDamage));
	public StatEntry<int> RawRitualDamage { get; } = InjectStatEntry<int>(new(10, StatDefinition<int>.RitualDamage));
	public StatEntry<float> AttackSpeed { get; } = InjectStatEntry<float>(new(1f, StatDefinition<float>.AttackSpeed));
	public StatEntry<float> CritChance { get; } = InjectStatEntry<float>(new(0.05f, StatDefinition<float>.CritChance));
	public StatEntry<float> CritMultiplier { get; } = InjectStatEntry<float>(new(1.5f, StatDefinition<float>.CritMultiplier));
	public StatEntry<float> Luck { get; } = InjectStatEntry<float>(new(0f, StatDefinition<float>.Luck));
	public StatEntry<float> LootBonus { get; } = InjectStatEntry<float>(new(0f, StatDefinition<float>.LootBonus));
	public float HorizontalAcceleration { get; set; } = 1f;
	public float HorizontalFlightAcceleration { get; set; } = 3f;
	public float VerticalFlightAcceleration { get; set; } = 2f;
	public int GrantedFlightTime { get; set; } = 0;

	static StatEntry<TValue> InjectStatEntry<TValue>(StatEntry<TValue> statEntry) where TValue : struct, IComparable<TValue> {
		registeredStats[statEntry.definitionReference] = statEntry;
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
			if (registeredStats.TryGetValue(stat.GetStatDefinition(), out var statEntry)) {
				statAction.Invoke(statEntry, stat, source);
			} else {
				throw new InvalidStatTypeReferenceBindingException(stat);
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

	private class InvalidStatTypeReferenceBindingException : NullReferenceException {
		public InvalidStatTypeReferenceBindingException(AbstractSerializableStat stat)
			: base ($"SerializableStat {stat.GetStatDefinition()} does not contain a valid stat binding. Could not find stat type reference {stat.GetStatDefinition()}") { }
	}
}


