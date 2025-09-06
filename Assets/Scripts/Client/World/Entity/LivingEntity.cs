using System;
using UnityEngine;

#nullable enable

public abstract class LivingEntity : Entity {
	protected float maxHealth;
	protected float currentHealth;
	public float CurrentHealth => currentHealth;
	private float immunityTimer;
	public bool isImmune { get; protected set; }

	public override void Spawn(EntitySpawnData spawnData) {
		base.Spawn(spawnData);
		this.maxHealth = spawnData.Get<float>("maxHealth");
		this.currentHealth = maxHealth;
	}

	public override void EntityUpdate(float deltaTime) {
		immunityTimer = Mathf.Clamp(immunityTimer - deltaTime, 0f, immunityTimer);
		isImmune = immunityTimer > 0;
	}

	public bool TakeDamage(float damage) {
		if (!CheckDeath(() => this.currentHealth -= damage) && !this.isImmune) {
			this.OnDamageTaken(damage);
		}
		return !this.isImmune;
	}

	public void SetHealth(float health) {
		CheckDeath(() => this.currentHealth = health);
	}

	public void SetMaxHealth(float maxHealth) {
		CheckDeath(() => this.maxHealth = maxHealth);
	}

	public void HandleDeath() {
		//...?
		this.OnDeath();
	}

	public void GrantImmunity(float seconds) {
		this.isImmune = true;
		this.immunityTimer = seconds;
	}

	private bool CheckDeath(Action? action) {
		action?.Invoke();
		if (maxHealth < currentHealth) {
			currentHealth = maxHealth;
		}
		if (currentHealth <= 0) {
			this.HandleDeath();
			return true;
		}
		return false;
	} 

	public abstract void OnDeath();

	public abstract void OnDamageTaken(float damage);

	public override SerializedEntityPropertyList GetSerializedProperties() {
		return new SerializedEntityPropertyList()
			.Add("maxHealth", maxHealth)
			.Add("currentHealth", currentHealth)
			.Add("immunityTimer", immunityTimer);
	}

	public override void ApplySerializedProperties(SerializedEntityPropertyList properties) {
		this.maxHealth = properties.Get<float>("maxHealth");
		this.currentHealth = properties.Get<float>("currentHealth");
		this.immunityTimer = properties.Get<float>("immunityTimer");
	}
}
