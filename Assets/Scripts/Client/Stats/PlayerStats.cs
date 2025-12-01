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
	public sealed class PlayerStats : IStatOwner, IStatModificationHost {
		private static readonly Logger logger = Logger.CreateInstance();
		private static readonly LogModule playerStats = new LogModule("PLAYER STATS", "#00FFFF");

		private readonly Dictionary<IStatDefinition, IStatEntry> entries = new();

		// REMINDER: current stat default values are subject to change
		public StatEntry<int> maxHealth = new(Stats.maxHealth, 100);
		public StatEntry<int> maxMana = new(Stats.maxMana, 50);
		public StatEntry<int> defense = new(Stats.defense, 0);
		public StatEntry<int> soulSlots = new(Stats.soulSlots, 2);
		public StatEntry<float> movementSpeed = new(Stats.movementSpeed, 8f);
		public StatEntry<int> jumpHeight = new(Stats.jumpHeight, 1);
		public StatEntry<float> dashVelocity = new(Stats.dashVelocity, 1f);
		public StatEntry<float> dashCooldown = new(Stats.dashCooldown, 2f);
		public StatEntry<float> healthRegen = new(Stats.healthRegen, 1.5f);
		public StatEntry<float> manaRegen = new(Stats.manaRegen, 2f);
		public StatEntry<int> rawPhysicalDamage = new(Stats.physicalDamage, 10);
		public StatEntry<int> rawRitualDamage = new(Stats.ritualDamage, 10);
		public StatEntry<float> attackSpeed = new(Stats.attackSpeed, 1f);
		public StatEntry<float> critChance = new(Stats.critChance, 0.05f);
		public StatEntry<float> critMultiplier = new(Stats.critMultiplier, 1.5f);
		public StatEntry<float> luck = new(Stats.luck, 0f);
		public StatEntry<float> lootBonus = new(Stats.lootBonus, 0f);
		public int maxJumps = 1;
		public float horizontalAcceleration = 1f;
		public float horizontalFlightAcceleration = 3f;
		public float verticalFlightAcceleration = 2f;
		public int grantedFlightTime = 0;

		public PlayerStats() {
			RegisterEntry(maxHealth);
			RegisterEntry(maxMana);
			RegisterEntry(defense);
			RegisterEntry(soulSlots);
			RegisterEntry(movementSpeed);
			RegisterEntry(jumpHeight);
			RegisterEntry(dashVelocity);
			RegisterEntry(dashCooldown);
			RegisterEntry(healthRegen);
			RegisterEntry(manaRegen);
			RegisterEntry(rawPhysicalDamage);
			RegisterEntry(rawRitualDamage);
			RegisterEntry(attackSpeed);
			RegisterEntry(critChance);
			RegisterEntry(critMultiplier);
			RegisterEntry(luck);
			RegisterEntry(lootBonus);
		}

		private void RegisterEntry(IStatEntry entry) {
			entries[entry.definition] = entry;
		}

		public bool TryGetEntry(IStatDefinition definition, out IStatEntry entry) {
			return entries.TryGetValue(definition, out entry);
		}

		public IReadOnlyDictionary<IStatDefinition, IStatEntry> GetEntries() => entries;

		[Obsolete]
		public void UpdateInjectedMappings() {
			//this.MaxHealth = (StatEntry<int>)injected[StatDefinition.MaxHealth];
			//this.MaxMana = (StatEntry<int>)injected[StatDefinition.MaxMana];
			//this.Defense = (StatEntry<int>)injected[StatDefinition.Defense];
			//this.SoulSlots = (StatEntry<int>)injected[StatDefinition.SoulSlots];
			//this.MovementSpeed = (StatEntry<float>)injected[StatDefinition.MovementSpeed];
			//this.JumpHeight = (StatEntry<int>)injected[StatDefinition.JumpHeight];
			//this.DashVelocity = (StatEntry<float>)injected[StatDefinition.DashVelocity];
			//this.DashCooldown = (StatEntry<float>)injected[StatDefinition.DashCooldown];
			//this.HealthRegen = (StatEntry<float>)injected[StatDefinition.HealthRegen];
			//this.ManaRegen = (StatEntry<float>)injected[StatDefinition.ManaRegen];
			//this.RawPhysicalDamage = (StatEntry<int>)injected[StatDefinition.PhysicalDamage];
			//this.RawRitualDamage = (StatEntry<int>)injected[StatDefinition.RitualDamage];
			//this.AttackSpeed = (StatEntry<float>)injected[StatDefinition.AttackSpeed];
			//this.CritChance = (StatEntry<float>)injected[StatDefinition.CritChance];
			//this.CritMultiplier = (StatEntry<float>)injected[StatDefinition.CritMultiplier];
			//this.Luck = (StatEntry<float>)injected[StatDefinition.Luck];
			//this.LootBonus = (StatEntry<float>)injected[StatDefinition.LootBonus];
		}

		public void ApplyModifiers(IStatModificationSource source) {
			foreach (var package in source.GetPackages()) {
				if (!entries.TryGetValue(package.definition, out var entry)) {
					continue;
				}
				foreach (var modifier in package.modifiers) {
					entry.AcceptModifier(modifier, source.token);
				}
			}
		}

		public void RemoveModifiers(IStatModificationSource source) {
			foreach (var package in source.GetPackages()) {
				if (!entries.TryGetValue(package.definition, out var entry)) {
					continue;
				}
				entry.RemoveModifiers(source.token);
			}
		}

	}
}
