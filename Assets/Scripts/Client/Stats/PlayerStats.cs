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
		[JsonConverter(typeof(JsonDictionaryConverter<IStatDefinitionImpl, IStatEntryImpl>))]
		private static Dictionary<IStatDefinitionImpl, IStatEntryImpl> injected = new();


		// REMINDER: current stat default values are subject to change
		[JsonIgnore] public StatEntry<int> MaxHealth { get; private set; } = InjectStatEntry<int>(new(200, StatDefinition.MaxHealth));
		[JsonIgnore] public StatEntry<int> MaxMana { get; private set; } = InjectStatEntry<int>(new(50, StatDefinition.MaxMana));
		[JsonIgnore] public StatEntry<int> Defense { get; private set; } = InjectStatEntry<int>(new(0, StatDefinition.Defense));
		[JsonIgnore] public StatEntry<int> SoulSlots { get; private set; } = InjectStatEntry<int>(new(2, StatDefinition.SoulSlots));
		[JsonIgnore] public StatEntry<float> MovementSpeed { get; private set; } = InjectStatEntry<float>(new(8f, StatDefinition.MovementSpeed));
		[JsonIgnore] public StatEntry<int> JumpHeight { get; private set; } = InjectStatEntry<int>(new(1, StatDefinition.JumpHeight));
		public int MaxJumps { get; set; } = 1;
		[JsonIgnore] public StatEntry<float> DashVelocity { get; private set; } = InjectStatEntry<float>(new(1f, StatDefinition.DashVelocity));
		[JsonIgnore] public StatEntry<float> DashCooldown { get; private set; } = InjectStatEntry<float>(new(2f, StatDefinition.DashCooldown));
		[JsonIgnore] public StatEntry<float> HealthRegen { get; private set; } = InjectStatEntry<float>(new(1.5f, StatDefinition.HealthRegen));
		[JsonIgnore] public StatEntry<float> ManaRegen { get; private set; } = InjectStatEntry<float>(new(2f, StatDefinition.ManaRegen));
		[JsonIgnore] public StatEntry<int> RawPhysicalDamage { get; private set; } = InjectStatEntry<int>(new(10, StatDefinition.PhysicalDamage));
		[JsonIgnore] public StatEntry<int> RawRitualDamage { get; private set; } = InjectStatEntry<int>(new(10, StatDefinition.RitualDamage));
		[JsonIgnore] public StatEntry<float> AttackSpeed { get; private set; } = InjectStatEntry<float>(new(1f, StatDefinition.AttackSpeed));
		[JsonIgnore] public StatEntry<float> CritChance { get; private set; } = InjectStatEntry<float>(new(0.05f, StatDefinition.CritChance));
		[JsonIgnore] public StatEntry<float> CritMultiplier { get; private set; } = InjectStatEntry<float>(new(1.5f, StatDefinition.CritMultiplier));
		[JsonIgnore] public StatEntry<float> Luck { get; private set; } = InjectStatEntry<float>(new(0f, StatDefinition.Luck));
		[JsonIgnore] public StatEntry<float> LootBonus { get; private set; } = InjectStatEntry<float>(new(0f, StatDefinition.LootBonus));
		public float HorizontalAcceleration { get; set; } = 1f;
		public float HorizontalFlightAcceleration { get; set; } = 3f;
		public float VerticalFlightAcceleration { get; set; } = 2f;
		public int GrantedFlightTime { get; set; } = 0;

		internal static StatEntry<TValue> InjectStatEntry<TValue>(StatEntry<TValue> statEntry) where TValue : struct, IComparable<TValue> {
			injected[statEntry.definitionReference] = statEntry;
			return statEntry;
		}

		public void ApplyStats(IEnumerable<AbstractSerializableStat> stats, IStatProvider receiver) {
			foreach (var stat in stats) {
				if (injected.TryGetValue(stat.GetStatDefinition(), out var statEntry)) {
					statEntry.Add(stat, receiver);
					UnityEngine.Debug.Log("added stat: " + stat.GetHashCode() + ": " + stat);
				} else {
					logger.LogError(null, new ArgumentException($"Could not apply stat to player receiver: unknown player stat definition {stat.GetStatDefinition()}"));
				}
			}
		}

		public void RevokeStats(IEnumerable<AbstractSerializableStat> stats, IStatProvider receiver) {
			foreach (var stat in stats) {
				if (injected.TryGetValue(stat.GetStatDefinition(), out var statEntry)) {
					statEntry.Remove(stat, receiver);
					UnityEngine.Debug.Log("removed stat: " + stat.GetHashCode() + ": " + stat);
				} else {
					logger.LogError(null, new ArgumentException($"Could not revoke stat to player receiver: unknown player stat definition {stat.GetStatDefinition()}"));
				}
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
