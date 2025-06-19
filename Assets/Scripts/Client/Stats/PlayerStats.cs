using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngineInternal;

public class PlayerStats {
	private static readonly Dictionary<IStatTypeImpl, IStatEntry> registeredStats = new();

	// REMINDER: current stat default values are subject to change
	public SimpleStatEntry<FlatIntStatProcessor, int> MaxHealth { get; } = new(200, StatType<int>.MaxHealth, RegisterStatInstance);
	public SimpleStatEntry<FlatIntStatProcessor, int> MaxMana { get; } = new(50, StatType<int>.MaxMana, RegisterStatInstance);
	public SimpleStatEntry<FlatIntStatProcessor, int> Defense { get; } = new(0, StatType<int>.Defense, RegisterStatInstance);
	public SimpleStatEntry<FlatIntStatProcessor, int> SoulSlots { get; } = new(2, StatType<int>.SoulSlots, RegisterStatInstance);
	public SimpleStatEntry<PercentageStatProcessor, float> MovementSpeed { get; } = new(8f, StatType<float>.MovementSpeed, RegisterStatInstance);
	public SimpleStatEntry<FlatIntStatProcessor, int> JumpHeight { get; } = new(1, StatType<int>.JumpHeight, RegisterStatInstance);
	public int MaxJumps { get; set; } = 1;
	public SimpleStatEntry<PercentageStatProcessor, float> DashVelocity { get; } = new(1f, StatType<float>.DashVelocity, RegisterStatInstance);
	public SimpleStatEntry<PercentageStatProcessor, float> DashCooldown { get; } = new(2f, StatType<float>.DashCooldown, RegisterStatInstance);
	public MultiplicativeStatEntry<MultiplicativeFloatStatProcessor, float> HealthRegen { get; } = new(1.5f, StatType<float>.HealthRegen, RegisterStatInstance);
	public MultiplicativeStatEntry<MultiplicativeFloatStatProcessor, float> ManaRegen { get; } = new(2f, StatType<float>.ManaRegen, RegisterStatInstance);
	public MultiplicativeStatEntry<MultiplicativeIntStatProcessor, int> RawPhysicalDamage { get; } = new(10, StatType<int>.PhysicalDamage, RegisterStatInstance);
	public MultiplicativeStatEntry<MultiplicativeIntStatProcessor, int> RawRitualDamage { get; } = new(10, StatType<int>.RitualDamage, RegisterStatInstance);
	public SimpleStatEntry<PercentageStatProcessor, float> AttackSpeed { get; } = new(1f, StatType<float>.AttackSpeed, RegisterStatInstance);
	public SimpleStatEntry<PercentageStatProcessor, float> CritChance { get; } = new(0.05f, StatType<float>.CritChance, RegisterStatInstance);
	public SimpleStatEntry<FlatFloatStatProcessor, float> CritMultiplier { get; } = new(1.5f, StatType<float>.CritMultiplier, RegisterStatInstance);
	public SimpleStatEntry<PercentageStatProcessor, float> Luck { get; } = new(0f, StatType<float>.Luck, RegisterStatInstance);
	public SimpleStatEntry<PercentageStatProcessor, float> LootBonus { get; } = new(0f, StatType<float>.LootBonus, RegisterStatInstance);
	public float HorizontalAcceleration { get; set; } = 1f;
	public float HorizontalFlightAcceleration { get; set; } = 3f;
	public float VerticalFlightAcceleration { get; set; } = 2f;
	public int GrantedFlightTime { get; set; } = 0;

	static void RegisterStatInstance(IStatTypeImpl statType, IStatEntry statEntry) => registeredStats[statType] = statEntry;

	public void Apply(List<SerializableStat> stats) {
		stats.ForEach(static stat => {
			if (registeredStats.TryGetValue(stat.SerializedReference.ToStatType(), out var statEntry)) {
				statEntry.ApplyToSerialized(stat);
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
}


