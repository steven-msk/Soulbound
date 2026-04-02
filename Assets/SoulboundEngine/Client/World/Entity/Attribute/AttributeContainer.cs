using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public sealed class AttributeContainer {
		private readonly IReadOnlyDictionary<AttributeType, object> baseValues;
		private readonly IReadOnlyDictionary<AttributeType, IValueRule?> ruleOverrides;
		private readonly Dictionary<AttributeType, List<IAttributeModifier>> modifiers = new();

		public AttributeContainer(
				IReadOnlyDictionary<AttributeType, object> baseValues,
				IReadOnlyDictionary<AttributeType, IValueRule?> ruleOverrides
			) {
			this.baseValues = baseValues;
			this.ruleOverrides = ruleOverrides;
		}

		public bool TryGetValue<T>(AttributeType<T> type, out T value) {
			T baseValue = default;
			bool typeIsDefined = false;
			if (baseValues.TryGetValue(type, out var val)) {
				baseValue = (T)val;
				typeIsDefined = true;
			}

			List<IAttributeModifier> modifiers = new();
			if (this.modifiers.TryGetValue(type, out var modifierList)) {
				modifiers = modifierList;
				typeIsDefined = true;
			}

			IValueRule? ruleOverride = ruleOverrides.GetValueOrDefault(type);
			value = (T)type.ComputeValue(baseValue, ruleOverride, modifiers);

			return typeIsDefined;
		}

		public void AddModifier<T>(AttributeType<T> type, IAttributeModifier<T> modifier) {
			if (!modifiers.ContainsKey(type)) {
				modifiers[type] = new List<IAttributeModifier>();
			}
			modifiers[type].Add(modifier);
		}

		public void RemoveModifier<T>(AttributeType<T> type, IAttributeModifier<T> modifier) {
			if (modifiers.TryGetValue(type, out var modifierList)) {
				modifierList.Remove(modifier);
			}
		}
	}
}
