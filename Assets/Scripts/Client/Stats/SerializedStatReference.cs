using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public enum SerializedStatReference {
	[StatBinding(typeof(StatDefinition<int>), nameof(StatDefinition<int>.MaxHealth))] MaxHealth,
	[StatBinding(typeof(StatDefinition<int>), nameof(StatDefinition<int>.MaxMana))] MaxMana,
	[StatBinding(typeof(StatDefinition<int>), nameof(StatDefinition<int>.Defense))] Defense,
	[StatBinding(typeof(StatDefinition<int>), nameof(StatDefinition<int>.SoulSlots))] SoulSlots,
	[StatBinding(typeof(StatDefinition<float>), nameof(StatDefinition<float>.MovementSpeed))] MovementSpeed,
	[StatBinding(typeof(StatDefinition<float>), nameof(StatDefinition<float>.DashVelocity))] DashVelocity,
	[StatBinding(typeof(StatDefinition<float>), nameof(StatDefinition<float>.DashCooldown))] DashCooldown,
	[StatBinding(typeof(StatDefinition<int>), nameof(StatDefinition<int>.JumpHeight))] JumpHeight,
	[StatBinding(typeof(StatDefinition<int>), nameof(StatDefinition<int>.MaxJumps))] MaxJumps,
	[StatBinding(typeof(StatDefinition<float>), nameof(StatDefinition<float>.HealthRegen))] HealthRegen,
	[StatBinding(typeof(StatDefinition<float>), nameof(StatDefinition<float>.ManaRegen))] ManaRegen,
	[StatBinding(typeof(StatDefinition<int>), nameof(StatDefinition<int>.PhysicalDamage))] PhysicalDamage,
	[StatBinding(typeof(StatDefinition<int>), nameof(StatDefinition<int>.RitualDamage))] RitualDamage,
	[StatBinding(typeof(StatDefinition<float>), nameof(StatDefinition<float>.AttackSpeed))] AttackSpeed,
	[StatBinding(typeof(StatDefinition<float>), nameof(StatDefinition<float>.CritChance))] CritChance,
	[StatBinding(typeof(StatDefinition<float>), nameof(StatDefinition<float>.CritMultiplier))] CritMultiplier,
	[StatBinding(typeof(StatDefinition<float>), nameof(StatDefinition<float>.Luck))] Luck,
	[StatBinding(typeof(StatDefinition<int>), nameof(StatDefinition<float>.LootBonus))] LootBonus,
}

public static class SerializedToInternalStatExtension {
	private static readonly Dictionary<SerializedStatReference, IStatDefinitionImpl> cached = new();

	public static IStatDefinitionImpl ToStatType(this SerializedStatReference serializedReference) {
		if (cached.TryGetValue(serializedReference, out var value)) {
			return value;
		}

		MemberInfo memberInfo = typeof(SerializedStatReference).GetMember(serializedReference.ToString()).FirstOrDefault();
		StatBindingAttribute attribute = memberInfo?.GetCustomAttribute<StatBindingAttribute>();
		FieldInfo field = attribute?.DeclaringType.GetField(attribute.FieldName, BindingFlags.Public | BindingFlags.Static);
		if (field == null || attribute == null || memberInfo == null) { 
			return null;
		}

		value = field.GetValue(null) as IStatDefinitionImpl;
		cached[serializedReference] = value;
		return value;
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	public static void ValidateStatEnumBindings() {
		foreach (SerializedStatReference serializedReference in Enum.GetValues(typeof(SerializedStatReference))) {
			if (serializedReference.ToStatType() == null) {
				UnityEngine.Debug.LogWarning($"Missing or invalid StatBinding for {serializedReference}");
			}
		}
	}
}
