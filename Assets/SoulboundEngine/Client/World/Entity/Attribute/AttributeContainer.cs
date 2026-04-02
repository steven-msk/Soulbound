using SoulboundEngine.Client.Debug.Logging;
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
			value = default!;
			bool typeIsDefined = false;
			if (baseValues.TryGetValue(type, out var baseValue)) {
				value = (T)baseValue;
				typeIsDefined = true;
			}

			if (modifiers.TryGetValue(type, out var modifierList)) {
				foreach (var modifier in modifierList) {
					object wrap = value;
					modifier.Apply(ref wrap);
					value = (T)wrap;
				}
				typeIsDefined = true;
			}

			IValueRule<T>? valueRule = ruleOverrides.TryGetValue(type, out IValueRule? boxed)
				? (IValueRule<T>?)boxed
				: type.GetValueRule();

			try {
				valueRule?.Apply(ref value);
			} catch (AttributeValueRuleViolationException e) {
				Logger.LogFatal(e);
				return false;
			}

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
