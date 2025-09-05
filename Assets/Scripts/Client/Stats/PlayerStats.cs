using System;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

public sealed class PlayerStats : IStatSource {
	private static readonly Logger logger = Logger.CreateInstance();
	private static readonly LogModule playerStats = new LogModule("PLAYER STATS", "#00FFFF");

	[JsonProperty]
	[JsonConverter(typeof(JsonDictionaryConverter<IStatDefinitionImpl, IStatEntryImpl>))]
	private static Dictionary<IStatDefinitionImpl, IStatEntryImpl> injected = new();

	// REMINDER: current stat default values are subject to change
	[JsonIgnore] public StatEntry<int> MaxHealth { get; private set; } = InjectStatEntry<int>(new(200, StatDefinition<int>.MaxHealth));
	[JsonIgnore] public StatEntry<int> MaxMana { get; private set; } = InjectStatEntry<int>(new(50, StatDefinition<int>.MaxMana));
	[JsonIgnore] public StatEntry<int> Defense { get; private set; } = InjectStatEntry<int>(new(0, StatDefinition<int>.Defense));
	[JsonIgnore] public StatEntry<int> SoulSlots { get; private set; } = InjectStatEntry<int>(new(2, StatDefinition<int>.SoulSlots));
	[JsonIgnore] public StatEntry<float> MovementSpeed { get; private set; } = InjectStatEntry<float>(new(8f, StatDefinition<float>.MovementSpeed));
	[JsonIgnore] public StatEntry<int> JumpHeight { get; private set; } = InjectStatEntry<int>(new(1, StatDefinition<int>.JumpHeight));
	public int MaxJumps { get; set; } = 1;
	[JsonIgnore] public StatEntry<float> DashVelocity { get; private set; } = InjectStatEntry<float>(new(1f, StatDefinition<float>.DashVelocity));
	[JsonIgnore] public StatEntry<float> DashCooldown { get; private set; } = InjectStatEntry<float>(new(2f, StatDefinition<float>.DashCooldown));
	[JsonIgnore] public StatEntry<float> HealthRegen { get; private set; } = InjectStatEntry<float>(new(1.5f, StatDefinition<float>.HealthRegen));
	[JsonIgnore] public StatEntry<float> ManaRegen { get; private set; } = InjectStatEntry<float>(new(2f, StatDefinition<float>.ManaRegen));
	[JsonIgnore] public StatEntry<int> RawPhysicalDamage { get; private set; } = InjectStatEntry<int>(new(10, StatDefinition<int>.PhysicalDamage));
	[JsonIgnore] public StatEntry<int> RawRitualDamage { get; private set; } = InjectStatEntry<int>(new(10, StatDefinition<int>.RitualDamage));
	[JsonIgnore] public StatEntry<float> AttackSpeed { get; private set; } = InjectStatEntry<float>(new(1f, StatDefinition<float>.AttackSpeed));
	[JsonIgnore] public StatEntry<float> CritChance { get; private set; } = InjectStatEntry<float>(new(0.05f, StatDefinition<float>.CritChance));
	[JsonIgnore] public StatEntry<float> CritMultiplier { get; private set; } = InjectStatEntry<float>(new(1.5f, StatDefinition<float>.CritMultiplier));
	[JsonIgnore] public StatEntry<float> Luck { get; private set; } = InjectStatEntry<float>(new(0f, StatDefinition<float>.Luck));
	[JsonIgnore] public StatEntry<float> LootBonus { get; private set; } = InjectStatEntry<float>(new(0f, StatDefinition<float>.LootBonus));
	public float HorizontalAcceleration { get; set; } = 1f;
	public float HorizontalFlightAcceleration { get; set; } = 3f;
	public float VerticalFlightAcceleration { get; set; } = 2f;
	public int GrantedFlightTime { get; set; } = 0;

	internal static StatEntry<TValue> InjectStatEntry<TValue>(StatEntry<TValue> statEntry) where TValue : struct, IComparable<TValue> {
		injected[statEntry.definitionReference] = statEntry;
		return statEntry;
	}

	public void ApplyProvider(IStatProvider provider) {
		foreach (var stat in provider.stats) {
			if (injected.TryGetValue(stat.GetStatDefinition(), out var statEntry)) {
				statEntry.Add(stat, provider);
			} else {
				logger.LogError(null, new ArgumentException($"Could not apply stat to player source: unknown player stat definition {stat.GetStatDefinition()}"));
			}
		}
	}

	public void RevokeProvider(IStatProvider provider) {
		foreach (var stat in provider.stats) {
			if (injected.TryGetValue(stat.GetStatDefinition(), out var statEntry)) {
				statEntry.Remove(stat, provider);
			} else {
				logger.LogError(null, new ArgumentException($"Could not revoke stat to player source: unknown player stat definition {stat.GetStatDefinition()}"));
			}
		}
	}

	public void UpdateInjectedMappings() {
		this.MaxHealth = (StatEntry<int>)injected[StatDefinition<int>.MaxHealth];
		this.MaxMana = (StatEntry<int>)injected[StatDefinition<int>.MaxMana];
		this.Defense = (StatEntry<int>)injected[StatDefinition<int>.Defense];
		this.SoulSlots = (StatEntry<int>)injected[StatDefinition<int>.SoulSlots];
		this.MovementSpeed = (StatEntry<float>)injected[StatDefinition<float>.MovementSpeed];
		this.JumpHeight = (StatEntry<int>)injected[StatDefinition<int>.JumpHeight];
		this.DashVelocity = (StatEntry<float>)injected[StatDefinition<float>.DashVelocity];
		this.DashCooldown = (StatEntry<float>)injected[StatDefinition<float>.DashCooldown];
		this.HealthRegen = (StatEntry<float>)injected[StatDefinition<float>.HealthRegen];
		this.ManaRegen = (StatEntry<float>)injected[StatDefinition<float>.ManaRegen];
		this.RawPhysicalDamage = (StatEntry<int>)injected[StatDefinition<int>.PhysicalDamage];
		this.RawRitualDamage = (StatEntry<int>)injected[StatDefinition<int>.RitualDamage];
		this.AttackSpeed = (StatEntry<float>)injected[StatDefinition<float>.AttackSpeed];
		this.CritChance = (StatEntry<float>)injected[StatDefinition<float>.CritChance];
		this.CritMultiplier = (StatEntry<float>)injected[StatDefinition<float>.CritMultiplier];
		this.Luck = (StatEntry<float>)injected[StatDefinition<float>.Luck];
		this.LootBonus = (StatEntry<float>)injected[StatDefinition<float>.LootBonus];
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


