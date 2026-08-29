namespace SoulboundEngine.Item {
	using Newtonsoft.Json.Linq;
	using SoulboundEngine.Component;
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Entity;
	using SoulboundEngine.World.Entity.Attribute;
	using System;
	using System.Collections;
	using System.Collections.Generic;

#nullable enable

	public record ItemAttributeModifiers(List<ItemAttributeModifiers.Entry> modifiers) : IEnumerable<ItemAttributeModifiers.Entry> {
		public static readonly ItemAttributeModifiers EMPTY = new(new List<Entry>());
		public static readonly ComponentType.Codec<ItemAttributeModifiers> CODEC = new(
			encoder: attributeModifiers => {
				JArray array = new();
				foreach (Entry entry in attributeModifiers) {
					array.Add(new JObject() {
						["attribute"] = entry.attribute.GetKey().value.ToString(),
						["modifier"] = entry.modifier.ToJson(),
						["slot"] = entry.slot.GetSerializedName()
					});
				}
				return array;
			},
			decoder: json => {
				if (json is not JArray array) {
					Logger.LogError("Item attribute modifiers json is not array: {}", json);
					return EMPTY;
				}

				Builder builder = Create();
				foreach (JToken token in array) {
					try {
						string idString = (string?)token["attribute"] ?? throw new NotSupportedException("No attribute on item attribute modifier entry json");
						Identifier id = Identifier.Of(idString);
						RegistryEntry<AttributeType> attribute = Registries.ATTRIBUTE.GetEntry(id) ?? throw new NotSupportedException("Unknown attribute " + idString);
						JToken modifierToken = token["modifier"] ?? throw new NotSupportedException("No modifier on item attribute modifier entry json");
						AttributeModifier modifier = AttributeModifier.FromJson(modifierToken);
						string slotString = (string?)token["slot"] ?? throw new NotSupportedException("No slot on item attribute modifier entry json");
						EquipmentSlot slot = EquipmentSlot.BySerializedName(slotString) ?? throw new NotSupportedException("Could not parse slot " + slotString);
						builder.Add(attribute, modifier, slot);
					} catch (Exception e) {
						Logger.LogFatal(e, "Failed to parse attribute modifier: {}", token);
					}
				}
				return builder.Build();
			}
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
