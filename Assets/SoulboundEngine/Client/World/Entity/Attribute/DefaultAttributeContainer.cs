using SoulboundEngine.Core.Registry;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public sealed class DefaultAttributeContainer {
		private readonly Dictionary<RegistryEntry<EntityAttribute>, AttributeInstance> instances = new();

		public DefaultAttributeContainer(Dictionary<RegistryEntry<EntityAttribute>, AttributeInstance> instances) {
			this.instances = instances;
		}

		public AttributeInstance CreateOverride(Action<AttributeInstance> updateCallback, RegistryEntry<EntityAttribute> attribute, IValueRule? ruleOverride = null) {
			bool hasDefault = instances.TryGetValue(attribute, out var defaultInstance);

			IValueRule? valueRule = ruleOverride ?? (hasDefault ? defaultInstance.GetValueRuleOverride() : null);
			AttributeInstance overrideInstance = new(attribute, updateCallback, valueRule) {
				baseValue = hasDefault ? defaultInstance.baseValue : 0d
			};

			return overrideInstance;
		}

		public double GetBaseValue(RegistryEntry<EntityAttribute> attribute) {
			return Require(attribute).baseValue;
		}

		public double GetModifierValue(RegistryEntry<EntityAttribute> attribute, Identifier identifier) {
			return Require(attribute).GetModifiers().First(m => m.IdMatches(identifier)).value;
		}

		public double GetValue(RegistryEntry<EntityAttribute> attribute) {
			return Require(attribute).GetValue();
		}

		public bool Has(RegistryEntry<EntityAttribute> attribute) => instances.ContainsKey(attribute);

		public bool HasModifier(RegistryEntry<EntityAttribute> attribute, Identifier identifier) {
			return Require(attribute).HasModifier(identifier);
		}

		private AttributeInstance Require(RegistryEntry<EntityAttribute> attribute) {
			return instances.TryGetValue(attribute, out AttributeInstance instance)
				? instance
				: throw new KeyNotFoundException($"Default attribute not found: {attribute.GetIdAsString()}");
		}

		public static Builder NewBuilder() => new();

		public class Builder {
			private readonly Dictionary<RegistryEntry<EntityAttribute>, AttributeInstance> instances = new();
			private bool unmodifiable;

			public Builder Add(RegistryEntry<EntityAttribute> attribute, IValueRule? ruleOverride = null) {
				if (unmodifiable) throw new InvalidOperationException();

				instances.Add(attribute, CheckedAdd(attribute, ruleOverride));
				return this;
			}

			public Builder Add(RegistryEntry<EntityAttribute> attribute, double baseValue, IValueRule? ruleOverride = null) {
				if (unmodifiable) throw new InvalidOperationException();

				AttributeInstance instance = CheckedAdd(attribute, ruleOverride);
				instances.Add(attribute, instance);
				instance.baseValue = baseValue;

				return this;
			}

			private AttributeInstance CheckedAdd(RegistryEntry<EntityAttribute> attribute, IValueRule? ruleOverride = null) {
				AttributeInstance instance = new(attribute, _ => { }, ruleOverride);
				return instance;
			}

			public DefaultAttributeContainer Build() {
				unmodifiable = true;
				return new DefaultAttributeContainer(instances);
			}
		}
	}
}
