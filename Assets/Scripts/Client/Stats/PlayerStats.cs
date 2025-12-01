using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Common.Json;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using Logger = SoulboundBackend.Common.Logging.Logger;

namespace SoulboundBackend.Client.Stats {
	public sealed class PlayerStats : IStatReceiver {
		private static readonly Logger logger = Logger.CreateInstance();
		private static readonly LogModule playerStats = new LogModule("PLAYER STATS", "#00FFFF");

		[JsonProperty]
		[JsonConverter(typeof(JsonDictionaryConverter<IStatDefinition, IStatEntry>))]
		private static Dictionary<IStatDefinition, IStatEntry> injected = new();

		// REMINDER: current stat default values are subject to change
		[JsonIgnore] public StatEntry<int> MaxHealth { get; private set; } = InjectStatEntry<int>(new(StatDefinition.MaxHealth, 100));
		[JsonIgnore] public StatEntry<int> MaxMana { get; private set; } = InjectStatEntry<int>(new(StatDefinition.MaxMana, 50));
		[JsonIgnore] public StatEntry<int> Defense { get; private set; } = InjectStatEntry<int>(new(StatDefinition.Defense, 0));
		[JsonIgnore] public StatEntry<int> SoulSlots { get; private set; } = InjectStatEntry<int>(new(StatDefinition.SoulSlots, 2));
		[JsonIgnore] public StatEntry<float> MovementSpeed { get; private set; } = InjectStatEntry<float>(new(StatDefinition.MovementSpeed, 8f));
		[JsonIgnore] public StatEntry<int> JumpHeight { get; private set; } = InjectStatEntry<int>(new(StatDefinition.JumpHeight, 1));
		public int MaxJumps { get; set; } = 1;
		[JsonIgnore] public StatEntry<float> DashVelocity { get; private set; } = InjectStatEntry<float>(new(StatDefinition.DashVelocity, 1f));
		[JsonIgnore] public StatEntry<float> DashCooldown { get; private set; } = InjectStatEntry<float>(new(StatDefinition.DashCooldown, 2f));
		[JsonIgnore] public StatEntry<float> HealthRegen { get; private set; } = InjectStatEntry<float>(new(StatDefinition.HealthRegen, 1.5f));
		[JsonIgnore] public StatEntry<float> ManaRegen { get; private set; } = InjectStatEntry<float>(new(StatDefinition.ManaRegen, 2f));
		[JsonIgnore] public StatEntry<int> RawPhysicalDamage { get; private set; } = InjectStatEntry<int>(new(StatDefinition.PhysicalDamage, 10));
		[JsonIgnore] public StatEntry<int> RawRitualDamage { get; private set; } = InjectStatEntry<int>(new(StatDefinition.RitualDamage, 10));
		[JsonIgnore] public StatEntry<float> AttackSpeed { get; private set; } = InjectStatEntry<float>(new(StatDefinition.AttackSpeed, 1f));
		[JsonIgnore] public StatEntry<float> CritChance { get; private set; } = InjectStatEntry<float>(new(StatDefinition.CritChance, 0.05f));
		[JsonIgnore] public StatEntry<float> CritMultiplier { get; private set; } = InjectStatEntry<float>(new(StatDefinition.CritMultiplier, 1.5f));
		[JsonIgnore] public StatEntry<float> Luck { get; private set; } = InjectStatEntry<float>(new(StatDefinition.Luck, 0f));
		[JsonIgnore] public StatEntry<float> LootBonus { get; private set; } = InjectStatEntry<float>(new(StatDefinition.LootBonus, 0f));
		public float HorizontalAcceleration { get; set; } = 1f;
		public float HorizontalFlightAcceleration { get; set; } = 3f;
		public float VerticalFlightAcceleration { get; set; } = 2f;
		public int GrantedFlightTime { get; set; } = 0;

		internal static StatEntry<TValue> InjectStatEntry<TValue>(StatEntry<TValue> statEntry) where TValue : struct, IComparable<TValue> {
			injected[statEntry.definition] = statEntry;
			return statEntry;
		}

		[Obsolete]
		public void ApplyStats(IEnumerable<AbstractValueModifier> stats, IStatProvider receiver) {
			foreach (var stat in stats) {
				//if (injected.TryGetValue(stat.statDefinition, out var statEntry)) {
				//	statEntry.Add(stat, receiver);
				//	UnityEngine.Debug.Log("added stat: " + stat.GetHashCode() + ": " + stat);
				//} else {
				//	logger.LogError(new ArgumentException($"Could not apply stat to player receiver: unknown player stat definition {stat.statDefinition}"));
				//}
			}
		}

		[Obsolete]
		public void RevokeStats(IEnumerable<AbstractValueModifier> stats, IStatProvider receiver) {
			foreach (var stat in stats) {
				//if (injected.TryGetValue(stat.statDefinition, out var statEntry)) {
				//	statEntry.Remove(stat, receiver);
				//	UnityEngine.Debug.Log("removed stat: " + stat.GetHashCode() + ": " + stat);
				//} else {
				//	logger.LogError(new ArgumentException($"Could not revoke stat to player receiver: unknown player stat definition {stat.statDefinition}"));
				//}
			}
		}

		public void UpdateInjectedMappings() {
			this.MaxHealth = (StatEntry<int>)injected[StatDefinition.MaxHealth];
			this.MaxMana = (StatEntry<int>)injected[StatDefinition.MaxMana];
			this.Defense = (StatEntry<int>)injected[StatDefinition.Defense];
			this.SoulSlots = (StatEntry<int>)injected[StatDefinition.SoulSlots];
			this.MovementSpeed = (StatEntry<float>)injected[StatDefinition.MovementSpeed];
			this.JumpHeight = (StatEntry<int>)injected[StatDefinition.JumpHeight];
			this.DashVelocity = (StatEntry<float>)injected[StatDefinition.DashVelocity];
			this.DashCooldown = (StatEntry<float>)injected[StatDefinition.DashCooldown];
			this.HealthRegen = (StatEntry<float>)injected[StatDefinition.HealthRegen];
			this.ManaRegen = (StatEntry<float>)injected[StatDefinition.ManaRegen];
			this.RawPhysicalDamage = (StatEntry<int>)injected[StatDefinition.PhysicalDamage];
			this.RawRitualDamage = (StatEntry<int>)injected[StatDefinition.RitualDamage];
			this.AttackSpeed = (StatEntry<float>)injected[StatDefinition.AttackSpeed];
			this.CritChance = (StatEntry<float>)injected[StatDefinition.CritChance];
			this.CritMultiplier = (StatEntry<float>)injected[StatDefinition.CritMultiplier];
			this.Luck = (StatEntry<float>)injected[StatDefinition.Luck];
			this.LootBonus = (StatEntry<float>)injected[StatDefinition.LootBonus];
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
}
