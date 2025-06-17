using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public enum SerializedStatReference {
	[StatBinding(typeof(StatType<int>), nameof(StatType<int>.MaxHealth))] MaxHealth,
	[StatBinding(typeof(StatType<int>), nameof(StatType<int>.MaxMana))] MaxMana,
	[StatBinding(typeof(StatType<int>), nameof(StatType<int>.Defense))] Defense,
	[StatBinding(typeof(StatType<int>), nameof(StatType<int>.SoulSlots))] SoulSlots,
	[StatBinding(typeof(StatType<float>), nameof(StatType<float>.MovementSpeed))] MovementSpeed,
	[StatBinding(typeof(StatType<float>), nameof(StatType<float>.DashVelocity))] DashVelocity,
	[StatBinding(typeof(StatType<float>), nameof(StatType<float>.DashCooldown))] DashCooldown,
	[StatBinding(typeof(StatType<int>), nameof(StatType<int>.JumpHeight))] JumpHeight,
	[StatBinding(typeof(StatType<int>), nameof(StatType<int>.MaxJumps))] MaxJumps,
	[StatBinding(typeof(StatType<float>), nameof(StatType<float>.HealthRegen))] HealthRegen,
	[StatBinding(typeof(StatType<float>), nameof(StatType<float>.ManaRegen))] ManaRegen,
	[StatBinding(typeof(StatType<int>), nameof(StatType<int>.PhysicalDamage))] PhysicalDamage,
	[StatBinding(typeof(StatType<int>), nameof(StatType<int>.RitualDamage))] RitualDamage,
	[StatBinding(typeof(StatType<float>), nameof(StatType<float>.AttackSpeed))] AttackSpeed,
	[StatBinding(typeof(StatType<float>), nameof(StatType<float>.CritChance))] CritChance,
	[StatBinding(typeof(StatType<float>), nameof(StatType<float>.CritMultiplier))] CritMultiplier,
	[StatBinding(typeof(StatType<float>), nameof(StatType<float>.Luck))] Luck,
	[StatBinding(typeof(StatType<float>), nameof(StatType<float>.LootBonus))] LootBonus
}

public static class SerializedToInternalStatExtension {
	private static readonly Dictionary<SerializedStatReference, IStatTypeImpl> cached = new();
	[CanBeNull] public static IStatTypeImpl ToStatType(this SerializedStatReference serializedReference) {
		if (cached.TryGetValue(serializedReference, out var statType)) {
			return statType;
		}

		MemberInfo memberInfo = typeof(SerializedStatReference).GetMember(serializedReference.ToString()).FirstOrDefault();
		StatBindingAttribute attribute = memberInfo.GetCustomAttribute<StatBindingAttribute>();
		FieldInfo field = attribute.DeclaringType.GetField(attribute.FieldName, BindingFlags.Public | BindingFlags.Static);
		if (field == null || attribute == null || memberInfo == null) { 
			return null;
		}

		IStatTypeImpl value = field.GetValue(null) as IStatTypeImpl;
		cached[serializedReference] = value;
		return value;
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	public static void ValidateStatEnumBindings() {
		foreach (SerializedStatReference serializedReference in Enum.GetValues(typeof(SerializedStatReference))) {
			if (serializedReference.ToStatType() == null) {
				Debug.LogWarning($"Missing or invalid StatBinding for {serializedReference}");
			}
		}
	}
}
