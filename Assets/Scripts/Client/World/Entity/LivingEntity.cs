using System;
using UnityEngine;

#nullable enable

public abstract class LivingEntity : Entity {
	private float maxHealth;
	private float currentHealth;
	public float CurrentHealth => currentHealth;

	public override void Spawn(EntitySpawnData spawnData) {
		base.Spawn(spawnData);
		this.maxHealth = spawnData.Get<float>("maxHealth");
		this.currentHealth = maxHealth;
	}

	public void TakeDamage(float damage) {
		if (!CheckDeath(() => this.currentHealth -= damage)) {
			this.OnDamageTaken(damage);
		}
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
}
