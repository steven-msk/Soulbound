using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngineInternal;

public class PlayerStats {
																																// default values (subject to major changes)
	public SimpleStatEntry<FlatIntStatProcessor, int> MaxHealth { get; } = new(200, StatType<int>.MaxHealth);                                            // 200
	public SimpleStatEntry<FlatIntStatProcessor, int> MaxMana { get; } = new(50, StatType<int>.MaxMana);                                                 // 50
	public SimpleStatEntry<FlatIntStatProcessor, int> Defense { get; } = new(0, StatType<int>.Defense);                                                  // 0
	public SimpleStatEntry<FlatIntStatProcessor, int> SoulSlots { get; } = new(2, StatType<int>.SoulSlots);                                              // 2
	public SimpleStatEntry<PercentageStatProcessor, float> MovementSpeed { get; } = new(8f, StatType<float>.MovementSpeed);                              // 8f
	public SimpleStatEntry<FlatIntStatProcessor, int> JumpHeight { get; } = new(1, StatType<int>.JumpHeight);                                            // 1
	public int MaxJumps { get; set; } = 1;																												 // 1
	public SimpleStatEntry<PercentageStatProcessor, float> DashVelocity { get; } = new(1f, StatType<float>.DashVelocity);                                // 1f
	public SimpleStatEntry<PercentageStatProcessor, float> DashCooldown { get; } = new(2f, StatType<float>.DashCooldown);                                // 2f
	public MultiplicativeStatEntry<MultiplicativeFloatStatProcessor, float> HealthRegen { get; } = new(1.5f, StatType<float>.HealthRegen);               // 1.5f
	public MultiplicativeStatEntry<MultiplicativeFloatStatProcessor, float> ManaRegen { get; } = new(2f, StatType<float>.ManaRegen);                     // 2f
	public MultiplicativeStatEntry<MultiplicativeIntStatProcessor, int> RawPhysicalDamage { get; } = new(10, StatType<int>.PhysicalDamage);              // 10
	public MultiplicativeStatEntry<MultiplicativeIntStatProcessor, int> RawRitualDamage { get; } = new(10, StatType<int>.RitualDamage);                  // 10
	public SimpleStatEntry<PercentageStatProcessor, float> AttackSpeed { get; } = new(1f, StatType<float>.AttackSpeed);                                  // 1f
	public SimpleStatEntry<PercentageStatProcessor, float> CritChance { get; } = new(0.05f, StatType<float>.CritChance);                                 // 0.05f
	public SimpleStatEntry<FlatFloatStatProcessor, float> CritMultiplier { get; } = new(1.5f, StatType<float>.CritMultiplier);                           // 1.5f
	public SimpleStatEntry<PercentageStatProcessor, float> Luck { get; } = new(0f, StatType<float>.Luck);                                                // 0f
	public SimpleStatEntry<PercentageStatProcessor, float> LootBonus { get; } = new(0f, StatType<float>.LootBonus);                                      // 0f
	public float HorizontalAcceleration { get; set; } = 1f;																								 // 1f
	public float HorizontalFlightAcceleration { get; set; } = 3f;																				         // 2f
	public float VerticalFlightAcceleration { get; set; } = 2f;																						     // 2f
	public int GrantedFlightTime { get; set; } = 0;																									     // 0

	//private float healthRegenAccumulator = 0;
	//private float manaRegenAccumulator = 0;

	// implement stat processing

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
