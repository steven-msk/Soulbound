namespace SoulboundEngine.Item {
	using SoulboundEngine.Component;
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Entity;
	using SoulboundEngine.World.Entity.Attribute;
	using System;
	using System.Collections.Generic;

	public record ItemAttributeModifiers(List<ItemAttributeModifiers.Entry> modifiers) {
		public static readonly ItemAttributeModifiers EMPTY = new(new List<Entry>());
		[Obsolete]
		public static readonly ComponentType.Codec<ItemAttributeModifiers> CODEC = new(
			encoder: attributeModifiers => {
				return default;
			},
			decoder: json => {
				return default;
			}
		);

		public static Builder Create() => new();

		public ItemAttributeModifiers With(RegistryEntry<AttributeType> attribute, AttributeModifier modifier, EquipmentSlot slot) {
			List<Entry> newModifiers = new(this.modifiers.Count);
			foreach (Entry entry in this.modifiers) {
				if (!entry.Matches(attribute, modifier.id)) {
					newModifiers.Add(entry);
				}
			}
			newModifiers.Add(new Entry(attribute, modifier, slot));
			return new ItemAttributeModifiers(newModifiers);
		}

		public void ForEach(EquipmentSlot slot, Action<RegistryEntry<AttributeType>, AttributeModifier> consumer) {
			foreach (Entry entry in this.modifiers) {
				if (entry.slot.Equals(slot)) {
					consumer(entry.attribute, entry.modifier);
				}
			}
		}

		public double Calculate(RegistryEntry<AttributeType> attribute, double baseValue, EquipmentSlot slot) {
			double flatSum = 0.0d;
			double percentSum = 0.0d;
			double multiplierProduct = 1.0d;

			foreach (Entry entry in this.modifiers) {
				if (entry.slot.Equals(slot) && entry.attribute == attribute) {
					double amount = entry.modifier.amount;
					
					if (entry.modifier.operation.Equals(AttributeModifier.Operation.ADDITIVE)) {
						flatSum += amount;
					} else if (entry.modifier.operation.Equals(AttributeModifier.Operation.ADDITIVE_PERCENT)) {
						percentSum += amount;
					} else if (entry.modifier.operation.Equals(AttributeModifier.Operation.MULTIPLICATIVE)) {
						multiplierProduct *= amount;
					}
				}	
			}

			return AttributeInstance.CalculateResult(baseValue, flatSum, percentSum, multiplierProduct);
		}

		public record Entry(RegistryEntry<AttributeType> attribute, AttributeModifier modifier, EquipmentSlot slot) {
			public bool Matches(RegistryEntry<AttributeType> attribute, Identifier id) {
				return attribute.Equals(this.attribute) && this.modifier.Matches(id);
			}
		}

		public sealed class Builder {
			private readonly List<Entry> entries = new();

			public Builder Add(RegistryEntry<AttributeType> attribute, AttributeModifier modifier, EquipmentSlot slot) {
				this.entries.Add(new Entry(attribute, modifier, slot));
				return this;
			}

			public ItemAttributeModifiers Build() => new(this.entries);
		}
	}
}
