namespace SoulboundEngine.World.Entity.Attribute {
	using SoulboundEngine.Registry;
	using System;
	using System.Collections.Generic;
	using System.Linq;

#nullable enable

	public class AttributeInstance {
		private readonly Dictionary<AttributeModifier.Operation, Dictionary<Identifier, AttributeModifier>> modifiersByOperation = new();
		private readonly Dictionary<Identifier, AttributeModifier> modifierById = new();
		private readonly Dictionary<Identifier, AttributeModifier> permanentModifiers = new();
		private readonly RegistryEntry<AttributeType> attribute;
		private readonly Action<AttributeInstance> onDirty;
		private double baseValue;
		private double cachedValue;
		private bool dirty = true;

		public AttributeInstance(RegistryEntry<AttributeType> attribute, Action<AttributeInstance> onDirty) {
			this.attribute = attribute;
			this.onDirty = onDirty;
			this.baseValue = attribute.GetValue().defaultValue;
		}

		public double GetBaseValue() => this.baseValue;

		public void SetBaseValue(double value) {
			if (this.baseValue != value) {
				this.baseValue = value;
				this.SetDirty();
			}
		}

		public RegistryEntry<AttributeType> GetAttribute() => this.attribute;

		public Dictionary<Identifier, AttributeModifier> GetModifiers(AttributeModifier.Operation operation) {
			if (this.modifiersByOperation.TryGetValue(operation, out Dictionary<Identifier, AttributeModifier> modifiers)) {
				return modifiers;
			}
			modifiers = new Dictionary<Identifier, AttributeModifier>();
			this.modifiersByOperation.Add(operation, modifiers);
			return modifiers;
		}

		public HashSet<AttributeModifier> GetModifiers() => this.modifierById.Values.ToHashSet();

		public HashSet<AttributeModifier> GetPermanentModifiers() => this.permanentModifiers.Values.ToHashSet();

		public AttributeModifier? GetModifier(Identifier id) => this.modifierById.GetValueOrDefault(id);

		public void AddModifier(AttributeModifier modifier) {
			if (!this.modifierById.TryAdd(modifier.id, modifier)) {
				throw new ArgumentException("Modifier is already applied on this attribute");
			}
			this.GetModifiers(modifier.operation).Add(modifier.id, modifier);
			this.SetDirty();
		}

		public void AddOrUpdateTransientModifier(AttributeModifier modifier) {
			if (this.modifierById.TryGetValue(modifier.id, out AttributeModifier old) && modifier == old) {
				return;
			}
			this.modifierById[modifier.id] = modifier;
			this.GetModifiers(modifier.operation)[modifier.id] = modifier;
			this.SetDirty();
		}

		public void AddTransientModifier(AttributeModifier modifier) {
			this.AddModifier(modifier);
		}

		public void AddOrReplacePermanentModifier(AttributeModifier modifier) {
			this.RemoveModifier(modifier.id);
			this.AddModifier(modifier);
			this.permanentModifiers[modifier.id] = modifier;
		}

		public void AddPermanentModifier(AttributeModifier modifier) {
			this.AddModifier(modifier);
			this.permanentModifiers.Add(modifier.id, modifier);
		}

		public void AddPermanentModifiers(IEnumerable<AttributeModifier> modifiers) {
			foreach (AttributeModifier modifier in modifiers) {
				this.AddPermanentModifier(modifier);
			}
		}

		protected void SetDirty() {
			this.dirty = true;
			this.onDirty(this);
		}

		public void RemoveModifier(AttributeModifier modifier) {
			this.RemoveModifier(modifier.id);
		}

		public bool RemoveModifier(Identifier id) {
			if (!this.modifierById.TryGetValue(id, out AttributeModifier modifier)) {
				return false;
			}
			this.GetModifiers(modifier.operation).Remove(id);
			this.permanentModifiers.Remove(id);
			this.SetDirty();
			return true;
		}

		public void RemoveModifiers() {
			foreach (AttributeModifier modifier in this.GetModifiers()) {
				this.RemoveModifier(modifier);
			}
		}

		public double GetValue() {
			if (this.dirty) {
				this.cachedValue = this.ComputeValue();
				this.dirty = false;
			}
			return this.cachedValue;
		}

		private double ComputeValue() {
			double value = this.GetBaseValue();
			foreach (AttributeModifier modifier in this.GetModifiersOrEmpty(AttributeModifier.Operation.ADDITIVE)) {
				value += modifier.amount;
			}

			double percentSum = 0.0d;
			foreach (AttributeModifier modifier in this.GetModifiersOrEmpty(AttributeModifier.Operation.ADDITIVE_PERCENT)) {
				percentSum += modifier.amount;
			}
			double result = value * (1.0d + percentSum);

			double multiplierProduct = 1.0d;
			foreach (AttributeModifier modifier in this.GetModifiersOrEmpty(AttributeModifier.Operation.MULTIPLICATIVE)) {
				multiplierProduct *= modifier.amount;
			}
			result *= multiplierProduct;

			return this.attribute.GetValue().ValidateValue(result);
		}

		private IEnumerable<AttributeModifier> GetModifiersOrEmpty(AttributeModifier.Operation operation) {
			if (this.modifiersByOperation.TryGetValue(operation, out Dictionary<Identifier, AttributeModifier> modifiers)) {
				foreach ((Identifier _, AttributeModifier modifier) in modifiers) {
					yield return modifier;
				}
			}
		}

		public void ReplaceFrom(AttributeInstance other) {
			this.baseValue = other.baseValue;
			this.modifierById.Clear();
			foreach ((Identifier id, AttributeModifier modifier) in other.modifierById) {
				this.modifierById.Add(id, modifier);
			}
			this.permanentModifiers.Clear();
			foreach ((Identifier id, AttributeModifier modifier) in other.permanentModifiers) {
				this.permanentModifiers.Add(id, modifier);
			}
			this.modifiersByOperation.Clear();
			foreach ((AttributeModifier.Operation operation, Dictionary<Identifier, AttributeModifier> modifiers) in other.modifiersByOperation) {
				Dictionary<Identifier, AttributeModifier> currentModifiers = this.GetModifiers(operation);
				foreach ((Identifier id, AttributeModifier modifier) in modifiers) {
					currentModifiers.Add(id, modifier);
				}
			}
			this.SetDirty();
		}

		public Packed Pack() => new(this.attribute, this.baseValue, new List<AttributeModifier>(this.permanentModifiers.Values));

		public void Apply(Packed packed) {
			this.baseValue = packed.baseValue;
			foreach (AttributeModifier modifier in packed.permanentModifiers) {
				this.modifierById[modifier.id] = modifier;
				this.GetModifiers(modifier.operation)[modifier.id] = modifier;
				this.permanentModifiers[modifier.id] = modifier;
			}
			this.SetDirty();
		}

		public record Packed(RegistryEntry<AttributeType> attribute, double baseValue, List<AttributeModifier> permanentModifiers);
	}
}
