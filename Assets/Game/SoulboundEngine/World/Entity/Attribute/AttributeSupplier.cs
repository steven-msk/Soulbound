namespace SoulboundEngine.World.Entity.Attribute {
	using SoulboundEngine.Registry;
	using System;
	using System.Collections.Generic;

#nullable enable

	public class AttributeSupplier {
		private readonly Dictionary<RegistryEntry<AttributeType>, AttributeInstance> instances = new();

		private AttributeSupplier(Dictionary<RegistryEntry<AttributeType>, AttributeInstance> instances) {
			this.instances = instances;
		}

		public static Builder Create() => new();

		private AttributeInstance GetInstance(RegistryEntry<AttributeType> attribute) {
			return !this.instances.TryGetValue(attribute, out AttributeInstance instance)
				? throw new ArgumentException("Cant find attribute " + attribute.GetIdAsString())
				: instance;
		}

		public double GetValue(RegistryEntry<AttributeType> attribute) {
			return this.GetInstance(attribute).GetValue();
		}

		public double GetBaseValue(RegistryEntry<AttributeType> attribute) {
			return this.GetInstance(attribute).GetBaseValue();
		}

		public double GetModifierValue(RegistryEntry<AttributeType> attribute, Identifier id) {
			return this.GetInstance(attribute).GetModifier(id)?.amount
				?? throw new ArgumentException("Cant find modifier " + id + " on attribute " + attribute.GetIdAsString());
		}

		public AttributeInstance? CreateInstance(Action<AttributeInstance> onDirty, RegistryEntry<AttributeType> attribute) {
			if (!this.instances.TryGetValue(attribute, out AttributeInstance template)) {
				return null;
			}
			AttributeInstance instance = new(attribute, onDirty);
			instance.ReplaceFrom(template);
			return instance;
		}

		public bool HasAttribute(RegistryEntry<AttributeType> attribute) => this.instances.ContainsKey(attribute);

		public bool HasModifier(RegistryEntry<AttributeType> attribute, Identifier modifier) {
			return this.instances.TryGetValue(attribute, out AttributeInstance instance) && instance.GetModifier(modifier) != null;
		}

		public sealed class Builder {
			private readonly Dictionary<RegistryEntry<AttributeType>, AttributeInstance> templates = new();
			private bool isFrozen;

			private AttributeInstance CreateAndAdd(RegistryEntry<AttributeType> attribute) {
				AttributeInstance result = new(attribute, instance => {
					if (this.isFrozen) {
						throw new InvalidOperationException("Attempted to change value for default attribute instance: " + attribute.GetIdAsString());
					}
				});
				this.templates[attribute] = result;
				return result;
			}

			public Builder Add(RegistryEntry<AttributeType> attribute) {
				this.CreateAndAdd(attribute);
				return this;
			}

			public Builder Add(RegistryEntry<AttributeType> attribute, double baseValue) {
				AttributeInstance instance = this.CreateAndAdd(attribute);
				instance.SetBaseValue(baseValue);
				return this;
			}

			public AttributeSupplier Build() {
				this.isFrozen = true;
				return new AttributeSupplier(this.templates);
			}
		}
	}
}
