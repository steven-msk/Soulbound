using SoulboundEngine.Core.Registry;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public sealed class AttributeContainer {
		private readonly Dictionary<RegistryEntry<EntityAttribute>, AttributeInstance> custom = new();
		private readonly DefaultAttributeContainer defaultAttributes;

		public AttributeContainer(DefaultAttributeContainer defaultAttributes) {
			this.defaultAttributes = defaultAttributes;
		}

		public void AddPersistentModifiersFrom(AttributeContainer other) {
			List<AttributeInstance.Packed> packedOther = other.Pack();
			Unpack(packedOther);
		}

		public double GetBaseValue(RegistryEntry<EntityAttribute> attribute) {
			return custom.TryGetValue(attribute, out AttributeInstance customInstance)
				? customInstance.baseValue
				: defaultAttributes.GetBaseValue(attribute);
		}

		public AttributeInstance? GetCustomInstance(RegistryEntry<EntityAttribute> attribute) {
			return custom.GetValueOrDefault(attribute);
		}

		public double GetModifierValue(RegistryEntry<EntityAttribute> attribute, Identifier identifier) {
			AttributeInstance? customInstance = GetCustomInstance(attribute);
			return customInstance != null
				? (customInstance.TryGetModifier(identifier, out AttributeModifier modifier)
					? modifier.value
					: 0d)
				: defaultAttributes.GetModifierValue(attribute, identifier);
		}

		public double GetValue(RegistryEntry<EntityAttribute> attribute) {
			AttributeInstance? customInstance = GetCustomInstance(attribute);
			return customInstance != null
				? customInstance.GetValue()
				: defaultAttributes.GetValue(attribute);
		}

		public bool HasAttribute(RegistryEntry<EntityAttribute> attribute) {
			return custom.ContainsKey(attribute) || defaultAttributes.Has(attribute);
		}

		public bool HasModifierForAttribute(RegistryEntry<EntityAttribute> attribute, Identifier identifier) {
			if (!custom.TryGetValue(attribute, out AttributeInstance instance)) return false;
			return instance.HasModifier(identifier);
		}

		public void ResetToBaseValue(RegistryEntry<EntityAttribute> attribute) {
			if (custom.TryGetValue(attribute, out AttributeInstance instance)) {
				instance.ClearModifiers();
			}
		}

		public void SetBaseFrom(AttributeContainer other) {
			foreach (var customAttribute in other.custom.Keys) {
				double baseValue = other.custom[customAttribute].baseValue;

				if (this.custom.TryGetValue(customAttribute, out AttributeInstance instance)) {
					instance.baseValue = baseValue;
				}
			}
		}

		public void SetFrom(AttributeContainer other) {
			SetBaseFrom(other);
			custom.Clear();
			foreach (var customAttribute in other.custom.Keys) {
				this.custom[customAttribute] = other.custom[customAttribute];
			}
		}

		public void RemoveModifiers(params (RegistryEntry<EntityAttribute> attribute, AttributeModifier modifier)[] modifiers) {
			foreach (var (attribute, modifier) in modifiers) {
				if (custom.TryGetValue(attribute, out AttributeInstance instance)) {
					instance.RemoveModifier(modifier);
				}
			}
		}

		public List<AttributeInstance.Packed> Pack() {
			List<AttributeInstance.Packed> packed = new();
			foreach (var attributeInstance in custom.Values) {
				packed.Add(attributeInstance.Pack());
			}
			return packed;
		}

		public void Unpack(List<AttributeInstance.Packed> packedList) {
			foreach (var packed in packedList) {
				AttributeInstance instance = defaultAttributes.CreateOverride(_ => { }, packed.entry, packed.ruleOverride);
				instance.Unpack(packed);
				if (!custom.TryAdd(packed.entry, instance)) {
					custom[packed.entry].AddPersistentModifiers(instance.GetPersistentModifiers().ToArray());
				}
			}
		}
	}
}
