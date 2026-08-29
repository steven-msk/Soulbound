namespace SoulboundEngine.World.Entity.Attribute {
	using SoulboundEngine.Registry;
	using System.Collections.Generic;

#nullable enable

	public class AttributeMap {
		private readonly Dictionary<RegistryEntry<AttributeType>, AttributeInstance> attributes = new();
		private readonly HashSet<AttributeInstance> attributesToUpdate = new();
		private readonly AttributeSupplier supplier;

		public AttributeMap(AttributeSupplier supplier) {
			this.supplier = supplier;
		}

		private void OnAttributeModified(AttributeInstance instance) {
			this.attributesToUpdate.Add(instance);
		}

		public HashSet<AttributeInstance> GetAttributesToUpdate() => this.attributesToUpdate;

		public AttributeInstance? GetInstance(RegistryEntry<AttributeType> attribute) {
			if (this.attributes.TryGetValue(attribute, out AttributeInstance? instance)) {
				return instance;
			}
			instance = this.supplier.CreateInstance(this.OnAttributeModified, attribute);
			if (instance != null) this.attributes.Add(attribute, instance);
			return instance;
		}

		public bool TryGetInstance(RegistryEntry<AttributeType> attribute, out AttributeInstance instance) {
			instance = this.GetInstance(attribute)!;
			return instance != null;
		}

		public bool HasAttribute(RegistryEntry<AttributeType> attribute) {
			return this.attributes.ContainsKey(attribute) || this.supplier.HasAttribute(attribute);
		}

		public bool HasModifier(RegistryEntry<AttributeType> attribute, Identifier modifier) {
			return this.attributes.GetValueOrDefault(attribute)?.GetModifier(modifier) != null || this.supplier.HasModifier(attribute, modifier);
		}

		public double GetValue(RegistryEntry<AttributeType> attribute) {
			return this.attributes.GetValueOrDefault(attribute)?.GetValue() ?? this.supplier.GetValue(attribute);
		}

		public double GetBaseValue(RegistryEntry<AttributeType> attribute) {
			return this.attributes.GetValueOrDefault(attribute)?.GetBaseValue() ?? this.supplier.GetBaseValue(attribute);
		}

		public double GetModifierValue(RegistryEntry<AttributeType> attribute, Identifier modifier) {
			return this.attributes.GetValueOrDefault(attribute)?.GetModifier(modifier)?.amount ?? this.supplier.GetModifierValue(attribute, modifier);
		}

		public void AddTransientModifiers(IDictionary<RegistryEntry<AttributeType>, List<AttributeModifier>> modifiers) {
			foreach ((RegistryEntry<AttributeType> attribute, List<AttributeModifier> attributeModifiers) in modifiers) {
				AttributeInstance? instance = this.GetInstance(attribute);
				if (instance == null) continue;

				foreach (AttributeModifier modifier in attributeModifiers) {
					instance.RemoveModifier(modifier.id);
					instance.AddTransientModifier(modifier);
				}
			}
		}

		public void RemoveAttributeModifiers(IDictionary<RegistryEntry<AttributeType>, List<AttributeModifier>> modifiers) {
			foreach ((RegistryEntry<AttributeType> attribute, List<AttributeModifier> attributeModifiers) in modifiers) {
				AttributeInstance? instance = this.GetInstance(attribute);
				if (instance == null) continue;

				foreach (AttributeModifier modifier in attributeModifiers) {
					instance.RemoveModifier(modifier.id);
				}
			}
		}

		public void AssignAllValues(AttributeMap other) {
			foreach (AttributeInstance instance in other.attributes.Values) {
				AttributeInstance? selfInstance = this.GetInstance(instance.GetAttribute());
				selfInstance?.ReplaceFrom(instance);
			}
		}

		public void AssignBaseValues(AttributeMap other) {
			foreach (AttributeInstance instance in other.attributes.Values) {
				AttributeInstance? selfInstance = this.GetInstance(instance.GetAttribute());
				selfInstance?.SetBaseValue(instance.GetBaseValue());
			}
		}

		public void AssignPermanentModifiers(AttributeMap other) {
			foreach (AttributeInstance instance in other.attributes.Values) {
				AttributeInstance? selfInstance = this.GetInstance(instance.GetAttribute());
				selfInstance?.AddPermanentModifiers(instance.GetPermanentModifiers());
			}
		}

		public bool ResetBaseValue(RegistryEntry<AttributeType> attribute) {
			if (!this.supplier.HasAttribute(attribute)) return false;

			AttributeInstance? instance = this.attributes.GetValueOrDefault(attribute);
			instance?.SetBaseValue(this.supplier.GetBaseValue(attribute));
			return true;
		}

		public List<AttributeInstance.Packed> Pack() {
			List<AttributeInstance.Packed> packed = new(this.attributes.Count);
			foreach (AttributeInstance instance in this.attributes.Values) {
				packed.Add(instance.Pack());
			}
			return packed;
		}

		public void Apply(List<AttributeInstance.Packed> packedAttributes) {
			foreach (AttributeInstance.Packed packed in packedAttributes) {
				AttributeInstance? instance = this.GetInstance(packed.attribute);
				instance?.Apply(packed);
			}
		}
	}
}
