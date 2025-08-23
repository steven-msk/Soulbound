using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngineInternal;

public class PlayerStats {
	private static readonly Dictionary<IStatDefinitionImpl, IStatEntry> registeredStats = new();

	// REMINDER: current stat default values are subject to change
	public SimpleStatEntry<FlatIntStatProcessor, int> MaxHealth { get; } = new(200, StatDefinition<int>.MaxHealth, RegisterStatInstance);
	public SimpleStatEntry<FlatIntStatProcessor, int> MaxMana { get; } = new(50, StatDefinition<int>.MaxMana, RegisterStatInstance);
	public SimpleStatEntry<FlatIntStatProcessor, int> Defense { get; } = new(0, StatDefinition<int>.Defense, RegisterStatInstance);
	public SimpleStatEntry<FlatIntStatProcessor, int> SoulSlots { get; } = new(2, StatDefinition<int>.SoulSlots, RegisterStatInstance);
	public SimpleStatEntry<PercentageStatProcessor, float> MovementSpeed { get; } = new(8f, StatDefinition<float>.MovementSpeed, RegisterStatInstance);
	public SimpleStatEntry<FlatIntStatProcessor, int> JumpHeight { get; } = new(1, StatDefinition<int>.JumpHeight, RegisterStatInstance);
	public int MaxJumps { get; set; } = 1;
	public SimpleStatEntry<PercentageStatProcessor, float> DashVelocity { get; } = new(1f, StatDefinition<float>.DashVelocity, RegisterStatInstance);
	public SimpleStatEntry<PercentageStatProcessor, float> DashCooldown { get; } = new(2f, StatDefinition<float>.DashCooldown, RegisterStatInstance);
	public MultiplicativeStatEntry<MultiplicativeFloatStatProcessor, float> HealthRegen { get; } = new(1.5f, StatDefinition<float>.HealthRegen, RegisterStatInstance);
	public MultiplicativeStatEntry<MultiplicativeFloatStatProcessor, float> ManaRegen { get; } = new(2f, StatDefinition<float>.ManaRegen, RegisterStatInstance);
	public MultiplicativeStatEntry<MultiplicativeIntStatProcessor, int> RawPhysicalDamage { get; } = new(10, StatDefinition<int>.PhysicalDamage, RegisterStatInstance);
	public MultiplicativeStatEntry<MultiplicativeIntStatProcessor, int> RawRitualDamage { get; } = new(10, StatDefinition<int>.RitualDamage, RegisterStatInstance);
	public SimpleStatEntry<PercentageStatProcessor, float> AttackSpeed { get; } = new(1f, StatDefinition<float>.AttackSpeed, RegisterStatInstance);
	public SimpleStatEntry<PercentageStatProcessor, float> CritChance { get; } = new(0.05f, StatDefinition<float>.CritChance, RegisterStatInstance);
	public SimpleStatEntry<FlatFloatStatProcessor, float> CritMultiplier { get; } = new(1.5f, StatDefinition<float>.CritMultiplier, RegisterStatInstance);
	public SimpleStatEntry<PercentageStatProcessor, float> Luck { get; } = new(0f, StatDefinition<float>.Luck, RegisterStatInstance);
	public SimpleStatEntry<PercentageStatProcessor, float> LootBonus { get; } = new(0f, StatDefinition<float>.LootBonus, RegisterStatInstance);
	public float HorizontalAcceleration { get; set; } = 1f;
	public float HorizontalFlightAcceleration { get; set; } = 3f;
	public float VerticalFlightAcceleration { get; set; } = 2f;
	public int GrantedFlightTime { get; set; } = 0;

	static void RegisterStatInstance(IStatDefinitionImpl statType, IStatEntry statEntry) => registeredStats[statType] = statEntry;

	public void Apply(List<AbstractSerializableStat> stats, IStatProvider source) {
		Invoke(stats, source, (statEntry, serializableStat, source) => statEntry.ApplyToSerialized(serializableStat, source));
	}

	public void Apply(AbstractSerializableStat stat, IStatProvider source) => Apply(new List<AbstractSerializableStat>() { stat }, source);

	public void Revoke(List<AbstractSerializableStat> stats, IStatProvider source) {
		Invoke(stats, source, (statEntry, serializableStat, source) => statEntry.RevokeToSerialized(serializableStat, source));
	}

	public void Revoke(AbstractSerializableStat stat, IStatProvider source) => Revoke(new List<AbstractSerializableStat> { stat }, source);

	private void Invoke(List<AbstractSerializableStat> stats, IStatProvider source, Action<IStatEntry, AbstractSerializableStat, IStatProvider> statAction) {
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


