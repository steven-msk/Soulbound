using SoulboundEngine.Core.Registry;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public sealed class AttributeContainer {
		private readonly Dictionary<RegistryEntry<EntityAttribute>, AttributeInstance> instances = new();
		private readonly DefaultAttributeContainer defaults;

		public AttributeContainer(DefaultAttributeContainer defaults) {
			this.defaults = defaults;

			foreach (var defaultInstance in defaults.GetEntries()) {
				RegistryEntry<EntityAttribute> attribute = defaultInstance.entry;
				AttributeInstance instance = defaults.CreateInstance(attribute, defaultInstance.ruleOverride);
				instances.Add(attribute, instance);
			}
		}

		public void AddPersistentModifiersFrom(AttributeContainer other) {
			foreach (var otherAttribute in other.instances.Keys) {
				AttributeInstance otherInstance = other.instances[otherAttribute];

				if (!instances.TryGetValue(otherAttribute, out AttributeInstance instance)) {
					instance = defaults.CreateInstance(otherAttribute, otherInstance.GetValueRuleOverride());
					instances[otherAttribute] = instance;
				}

				instance.AddPersistentModifiers(otherInstance.GetPersistentModifiers()
					.Where(m => !otherInstance.HasPredicateModifier(m.identifier))
					.ToArray());
				instance.AddPredicateModifiers(otherInstance.GetPredicateModifiers().ToArray());
			}
		}

		public double GetBaseValue(RegistryEntry<EntityAttribute> attribute) {
			return Require(attribute).baseValue;
		}

		public double GetModifierValue(RegistryEntry<EntityAttribute> attribute, Identifier identifier) {
			return Require(attribute).TryGetModifier(identifier, out var modifier)
				? modifier.value
				: 0d;
		}

		public double GetValue(RegistryEntry<EntityAttribute> attribute) {
			return Require(attribute).GetValue();
		}

		public bool HasAttribute(RegistryEntry<EntityAttribute> attribute) {
			return instances.ContainsKey(attribute);
		}

		public bool HasModifierForAttribute(RegistryEntry<EntityAttribute> attribute, Identifier identifier) {
			if (!instances.TryGetValue(attribute, out AttributeInstance instance)) return false;
			return instance.HasModifier(identifier);
		}

		public void ResetToBaseValue(RegistryEntry<EntityAttribute> attribute) {
			if (instances.TryGetValue(attribute, out AttributeInstance instance)) {
				instance.ClearModifiers();
			}
		}

		public void SetBaseFrom(AttributeContainer other) {
			foreach (var customAttribute in other.instances.Keys) {
				double baseValue = other.instances[customAttribute].baseValue;

				if (this.instances.TryGetValue(customAttribute, out AttributeInstance instance)) {
					instance.baseValue = baseValue;
				}
			}
		}

		public void SetFrom(AttributeContainer other) {
			SetBaseFrom(other);
			foreach (var attribute in other.instances.Keys) {
				AttributeInstance otherInstance = other.instances[attribute];

				if (!instances.TryGetValue(attribute, out AttributeInstance instance)) {
					instance = defaults.CreateInstance(attribute, otherInstance.GetValueRuleOverride());
					instances[attribute] = instance;
				}

				instance.ClearModifiers();
				instance.AddPersistentModifiers(otherInstance.GetPersistentModifiers()
					.Where(m => !otherInstance.HasPredicateModifier(m.identifier))
					.ToArray());
				instance.AddPredicateModifiers(otherInstance.GetPredicateModifiers().ToArray());
			}
		}

		public void RemoveModifiers(params (RegistryEntry<EntityAttribute> attribute, AttributeModifier modifier)[] modifiers) {
			foreach (var (attribute, modifier) in modifiers) {
				if (instances.TryGetValue(attribute, out AttributeInstance instance)) {
					instance.RemoveModifier(modifier);
				}
			}
		}

		private AttributeInstance Require(RegistryEntry<EntityAttribute> attribute) {
			return instances.TryGetValue(attribute, out var instance)
				? instance
				: throw new KeyNotFoundException(attribute.GetIdAsString());
		}
	}
}
