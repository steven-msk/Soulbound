namespace SoulboundEngine.Item {
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Entity;
	using SoulboundEngine.World.Entity.Attribute;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;

#nullable enable

	public record ItemAttributeModifiers(List<ItemAttributeModifiers.Entry> modifiers) : IEnumerable<ItemAttributeModifiers.Entry> {
		public static readonly ItemAttributeModifiers EMPTY = new(new List<Entry>());
		public static readonly Codec<ItemAttributeModifiers> CODEC = Entry.CODEC.ListOf().Xmap(
			list => {
				Builder builder = Create();
				foreach (Entry entry in list) {
					builder.Add(entry.attribute, entry.modifier, entry.slot);
				}
				return builder.Build();
			},
			modifiers => modifiers.ToList()
		);

		public static Builder Create() => new();

		public ItemAttributeModifiers With(RegistryEntry<AttributeType> attribute, AttributeModifier modifier, EquipmentSlot slot) {
			List<Entry> newModifiers = new(this.modifiers.Count);
			foreach (Entry entry in this) {
				if (!entry.Matches(attribute, modifier.id)) {
					newModifiers.Add(entry);
				}
			}
			newModifiers.Add(new Entry(attribute, modifier, slot));
			return new ItemAttributeModifiers(newModifiers);
		}

		public void ForEach(EquipmentSlot slot, Action<RegistryEntry<AttributeType>, AttributeModifier> consumer) {
			foreach (Entry entry in this) {
				if (entry.slot.Equals(slot)) {
					consumer(entry.attribute, entry.modifier);
				}
			}
		}

		public double Calculate(RegistryEntry<AttributeType> attribute, double baseValue, EquipmentSlot slot) {
			double flatSum = 0.0d;
			double percentSum = 0.0d;
			double multiplierProduct = 1.0d;

			foreach (Entry entry in this) {
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

		public IEnumerator<Entry> GetEnumerator() => this.modifiers.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		public record Entry(RegistryEntry<AttributeType> attribute, AttributeModifier modifier, EquipmentSlot slot) {
			public static readonly Codec<Entry> CODEC = RecordCodec<Entry, RegistryEntry<AttributeType>, AttributeModifier, EquipmentSlot>.Of(
				Field.Required<Entry, RegistryEntry<AttributeType>>("attribute", RegistryEntry<AttributeType>.GetCodec(Registries.ATTRIBUTE), e => e.attribute),
				Field.Required<Entry, AttributeModifier>("modifier", AttributeModifier.CODEC, e => e.modifier),
				Field.Required<Entry, EquipmentSlot>("slot", EquipmentSlot.CODEC, e => e.slot),
				(attribute, modifier, slot) => new Entry(attribute, modifier, slot)
			);

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
