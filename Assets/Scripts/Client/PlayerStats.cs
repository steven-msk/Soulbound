using System;
using UnityEngine;

[Serializable]
public struct PlayerStats {
	private int health;
	public int Health {
		readonly get => health;
		set => health = Mathf.Clamp(value, 0, maxHealth);
	}
	public int maxHealth;
	private int mana;
	public int Mana {
		readonly get => mana;
		set => mana = Mathf.Clamp(value, 0, maxMana);
	}
	public int maxMana;
	public int defense;
	public int soulSlots;
	public float movementSpeed;
	public float jumpHeight;
	public int maxJumps;
	public float dashCooldown;
	public float healthRegen;
	public float manaRegen;
	public float rawPhysicalDamage;
	public float rawRitualDamage;
	public float attackSpeed;
	public float critChance;
	public float critMultiplier;
	public float luck;
	public float lootBonus;
	public float horizontalAcceleration;
	public float horizontalFlightAcceleration;
	public float verticalFlightAcceleration;
	public float grantedFlightTime;

	private float healthRegenAccumulator;
	private float manaRegenAccumulator;

	public PlayerStats(
					int maxHealth = 100, int maxMana = 50, int defense = 0, int soulSlots = 2, int maxJumps = 1,
					float movementSpeed = 10f, float jumpHeight = 1f, float dashCooldown = 2f, float healthRegen = 1.5f,
					float manaRegen = 2f, float rawPhysicalDamage = 10f, float rawRitualDamage = 10f, float attackSpeed = 1f,
					float critChance = 0.05f, float critMultiplier = 1.5f, float luck = 0f, float lootBonus = 0f, float grantedFlightTime = 0f,
					float horizontalAcceleration = 1f, float horizontalFlightAcceleration = 1f, float verticalFlightAcceleration = 2f) {
		this.health = maxHealth;
		this.maxHealth = maxHealth;
		this.mana = maxMana;
		this.maxMana = maxMana;
		this.defense = defense;
		this.soulSlots = soulSlots;
		this.movementSpeed = movementSpeed;
		this.jumpHeight = jumpHeight;
		this.maxJumps = maxJumps;
		this.dashCooldown = dashCooldown;
		this.healthRegen = healthRegen;
		this.manaRegen = manaRegen;
		this.rawPhysicalDamage = rawPhysicalDamage;
		this.rawRitualDamage = rawRitualDamage;
		this.attackSpeed = attackSpeed;
		this.critChance = critChance;
		this.critMultiplier = critMultiplier;
		this.luck = luck;
		this.lootBonus = lootBonus;
		this.healthRegenAccumulator = 0f;
		this.manaRegenAccumulator = 0f;
		this.horizontalAcceleration = horizontalAcceleration;
		this.horizontalFlightAcceleration = horizontalFlightAcceleration;
		this.verticalFlightAcceleration = verticalFlightAcceleration;
		this.grantedFlightTime = grantedFlightTime;
	}

	// implement stat processing

	public void UpdateRegenStats() {
		healthRegenAccumulator += healthRegen * Time.deltaTime;
		if (healthRegenAccumulator >= 1f) {
			int regenAmount = Mathf.FloorToInt(healthRegenAccumulator);
			Health += regenAmount;
			healthRegenAccumulator -= regenAmount;
		}

		manaRegenAccumulator += manaRegen * Time.deltaTime;
		if (manaRegenAccumulator >= 1f) {
			int regenAmount = Mathf.FloorToInt(manaRegenAccumulator);
			Mana += regenAmount;
			manaRegenAccumulator -= regenAmount;
		}
	}
}