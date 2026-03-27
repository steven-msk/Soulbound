using System.Collections.Generic;

namespace SoulboundEngine.Client.Stats {
	public sealed class PlayerStats : IStatOwner, IStatModificationHost {
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

		public void ApplyModifiers(IStatModificationSource source) {
			foreach (var package in source.GetPackages()) {
				if (!entries.TryGetValue(package.definition, out var entry)) {
					continue;
				}
				foreach (var modifier in package.modifiers) {
					UnityEngine.Debug.Log("adding modifier: " + package.definition + $" [{modifier.GetHashCode()}]");
					entry.AcceptModifier(modifier, source.token);
				}
				UnityEngine.Debug.Log("entry value: " + entry.CalculateBoxedValue());
			}
		}

		public void RemoveModifiers(IStatModificationSource source) {
			foreach (var package in source.GetPackages()) {
				if (!entries.TryGetValue(package.definition, out var entry)) {
					continue;
				}
				foreach (var modifier in package.modifiers) {
					UnityEngine.Debug.Log("removing modifier: " + package.definition + $" [{modifier.GetHashCode()}]");
				}
				entry.RemoveModifiers(source.token);
				UnityEngine.Debug.Log("entry value: " + entry.CalculateBoxedValue());
			}
		}

	}
}
