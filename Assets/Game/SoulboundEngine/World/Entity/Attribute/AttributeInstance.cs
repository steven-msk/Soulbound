using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Registry;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SoulboundEngine.World.Entity.Attribute {
	public sealed class AttributeInstance {
		private readonly Dictionary<Identifier, AttributeModifier> idToModifier = new();
		private readonly Dictionary<Identifier, AttributeModifier> persistentModifiers = new();
		private readonly Dictionary<Identifier, Func<bool>> idToPredicate = new();
		private readonly Dictionary<Identifier, bool> lastPredicateState = new();
		private readonly RegistryEntry<EntityAttribute> type;
		private readonly IValueRule? ruleOverride;
		private double value;
		private bool dirty;

		public AttributeInstance(RegistryEntry<EntityAttribute> type, IValueRule? ruleOverride = null) {
			this.type = type;
			this.dirty = true;
			this.ruleOverride = ruleOverride;
		}

		public double baseValue { get; set; }

		private void AddModifier(AttributeModifier modifier) {
			this.idToModifier.Add(modifier.identifier, modifier);
			this.dirty = true;
		}

		public void AddPersistentModifier(AttributeModifier modifier) {
			this.AddModifier(modifier);
			this.persistentModifiers.Add(modifier.identifier, modifier);
		}
		public void AddPredicateModifier(AttributeModifier modifier, Func<bool> predicate) {
			this.AddPersistentModifier(modifier);
			this.idToPredicate.Add(modifier.identifier, predicate);
		}

		public void AddPersistentModifiers(params AttributeModifier[] modifiers) {
			foreach (var modifier in modifiers) {
				this.AddPersistentModifier(modifier);
			}
		}
		public void AddPredicateModifiers(params (AttributeModifier modifier, Func<bool> predicate)[] modifiers) {
			foreach (var (modifier, predicate) in modifiers) {
				this.AddPredicateModifier(modifier, predicate);
			}
		}

		public void ClearModifiers() {
			this.idToModifier.Clear();
			this.persistentModifiers.Clear();
			this.idToPredicate.Clear();
			this.dirty = true;
		}

		public void OnUpdate() {
			if (this.HasPredicateModifiers()) {
				bool anyChanged = false;

				foreach (var id in this.idToPredicate.Keys) {
					bool current = this.idToPredicate[id]();
					if (current != this.lastPredicateState.GetValueOrDefault(id, !current)) {
						anyChanged = true;
					}
					this.lastPredicateState[id] = current;
				}

				if (anyChanged) this.dirty = true;
			}
		}

		private double ComputeValue() {
			if (!this.dirty) return this.value;

			List<AttributeModifier> allModifiers = this.GetModifiers().ToList();

			// filter targeting modifiers
			IEnumerable<AttributeModifier> targeting = this.GetTargetingModifiers(allModifiers);
			allModifiers.RemoveAll(m => targeting.Contains(m));

			Dictionary<AttributeModifier, List<AttributeModifier>> modifierToItsTargeters = new();
			foreach (var targeter in targeting) {
				IEnumerable<AttributeModifier> targets = targeter.target!.Resolve(allModifiers);

				foreach (var target in targets) {
					if (!modifierToItsTargeters.ContainsKey(target)) {
						modifierToItsTargeters[target] = new List<AttributeModifier>();
					}
					modifierToItsTargeters[target].Add(targeter);
				}
			}

			// this design exposes a risk:
			// predicates, and especially targeting predicates,
			// may require another attribute's value.
			// if that attribute has predicate modifiers
			// which target the value of this attribute
			// it ends up in a recursive loop.
			// or even worse, a recursive cycle between multiple modifiers.
			// TODO: fix recursive cycle risk of predicate modifiers


			// calculate effective overrides from targeters
			Dictionary<AttributeModifier, double> effectiveOverrides = new();

			// TODO: fix unordered modifier graph lookup, which is dangerous for recursion
			foreach (var target in modifierToItsTargeters.Keys) {
				List<AttributeModifier> targeters = modifierToItsTargeters[target];
				List<AttributeModifier> predicate_targeters = this.GetPredicateModifiers(targeters).ToList();
				targeters.RemoveAll(m => predicate_targeters.Contains(m));

				double effectiveOverride = this.CalculateModifiedValue(target.value, targeters, _ => null);

				predicate_targeters.RemoveAll(m => !this.idToPredicate[m.identifier]());
				effectiveOverride = this.CalculateModifiedValue(effectiveOverride, predicate_targeters, _ => null);

				effectiveOverrides.Add(target, effectiveOverride);
			}

			// filter predicates
			HashSet<AttributeModifier> predicateModifiers = this.GetPredicateModifiers(allModifiers).ToHashSet();
			allModifiers.RemoveAll(m => predicateModifiers.Contains(m));
			double? EffectiveOverrideSupplier(AttributeModifier attribute) {
				return effectiveOverrides.TryGetValue(attribute, out double _override) ? _override : null;
			}
			double prePredicateResult = this.CalculateModifiedValue(this.baseValue, allModifiers, EffectiveOverrideSupplier);

			predicateModifiers.RemoveWhere(m => !this.idToPredicate[m.identifier]());
			double final = this.CalculateModifiedValue(prePredicateResult, predicateModifiers, EffectiveOverrideSupplier);

			// apply value rule
			try {
				IValueRule? valueRule = this.ruleOverride ?? this.type.GetValue().ValueRule;
				valueRule?.Apply(ref final);
			} catch (AttributeValueRuleViolationException e) {
				Logger.LogFatal(e);
				final = this.baseValue;
			} finally {
				this.value = final;
			}

			this.dirty = false;
			return this.value;
		}

		// default numeric value computation:
		// A = base + Σ(flat)
		// B = A * (1 + Σ%) (% of A)
		// C = B * Π(multipliers)
		private double CalculateModifiedValue(double baseValue, IEnumerable<AttributeModifier> modifiers, Func<AttributeModifier, double?> effectiveOverrideSupplier) {
			this.FilterOperations(modifiers,
				out List<AttributeModifier> additive,
				out List<AttributeModifier> additivePercent,
				out List<AttributeModifier> multiplicative
			);

			// apply all flat adds/subtracts (A)
			double A = baseValue;
			foreach (var modifier in additive) {
				modifier.Apply(effectiveOverrideSupplier(modifier), ref A);
			}

			// apply all percentage adds (B)
			double percentSum = 0d;
			foreach (var modifier in additivePercent) {
				modifier.Apply(effectiveOverrideSupplier(modifier), ref percentSum);
			}
			double B = A * (1d + percentSum);

			// apply multipliers (C)
			double multiplierProduct = 1d;
			foreach (var modifier in multiplicative) {
				modifier.Apply(effectiveOverrideSupplier(modifier), ref multiplierProduct);
			}
			double C = B * multiplierProduct;

			return C;
		}

		private void FilterOperations(
			IEnumerable<AttributeModifier> modifiers,
			out List<AttributeModifier> additive,
			out List<AttributeModifier> additivePercent,
			out List<AttributeModifier> multiplicative
		) {
			additive = new List<AttributeModifier>();
			additivePercent = new List<AttributeModifier>();
			multiplicative = new List<AttributeModifier>();

			foreach (var modifier in modifiers) {
				OperationType opType = modifier.GetOperationType();

				if (opType == OperationType.Additive) additive.Add(modifier);
				else if (opType == OperationType.AdditivePercent) additivePercent.Add(modifier);
				else if (opType == OperationType.Multiplicative) multiplicative.Add(modifier);
			}
		}

		public double GetValue() {
			if (this.dirty) this.ComputeValue();
			return this.value;
		}

		public IValueRule? GetValueRuleOverride() => this.ruleOverride;

		private bool IsPredicate(AttributeModifier modifier) => this.idToPredicate.ContainsKey(modifier.identifier);

		public RegistryEntry<EntityAttribute> GetAttribute() => this.type;

		public bool TryGetModifier(Identifier identifier, out AttributeModifier modifier) {
			return this.idToModifier.TryGetValue(identifier, out modifier);
		}

		public IEnumerable<AttributeModifier> GetModifiers() => this.idToModifier.Values.ToHashSet();
		public IReadOnlyDictionary<Identifier, AttributeModifier> GetModifiersByOperation(IOperation operation) {
			return this.idToModifier
				.Where(kvp => kvp.Value.operation.Equals(operation))
				.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
		}
		public bool HasModifier(Identifier identifier) => this.idToModifier.ContainsKey(identifier);
		public bool HasPredicateModifier(Identifier identifier) => this.idToPredicate.ContainsKey(identifier);
		public IEnumerable<AttributeModifier> GetPersistentModifiers() => this.persistentModifiers.Values.ToHashSet();
		private IEnumerable<AttributeModifier> GetTargetingModifiers(IEnumerable<AttributeModifier> modifiers) {
			return modifiers
				.Where(m => m.target != null);
		}
		private IEnumerable<AttributeModifier> GetPredicateModifiers(IEnumerable<AttributeModifier> modifiers) {
			return modifiers
				.Where(m => this.IsPredicate(m));
		}
		public IEnumerable<(AttributeModifier attribute, Func<bool> predicate)> GetPredicateModifiers() {
			return this.idToPredicate.Keys
				.Where(id => this.idToModifier.ContainsKey(id))
				.Select(id => (this.idToModifier[id], this.idToPredicate[id]));
		}
		public bool HasPredicateModifiers() => this.idToPredicate.Any();

		public void OverwritePersistentModifier(AttributeModifier modifier) {
			if (!this.idToModifier.ContainsKey(modifier.identifier)) return;

			this.idToModifier[modifier.identifier] = modifier;
			this.persistentModifiers[modifier.identifier] = modifier;
			this.idToPredicate.Remove(modifier.identifier);

			this.dirty = true;
		}
		public void OverwritePredicateModifier(AttributeModifier modifier, Func<bool> predicate) {
			this.OverwritePersistentModifier(modifier);
			this.idToPredicate.Add(modifier.identifier, predicate);
		}
		public void OverwritePredicate(Identifier identifier, Func<bool> predicate) {
			if (this.idToPredicate.ContainsKey(identifier)) {
				this.idToPredicate[identifier] = predicate;
			}
		}

		public void RemoveModifier(AttributeModifier modifier) => this.RemoveModifier(modifier.identifier);
		public void RemoveModifier(Identifier identifier) {
			this.idToModifier.Remove(identifier);
			this.persistentModifiers.Remove(identifier);
			this.idToPredicate.Remove(identifier);
			this.dirty = true;
		}

	}
}
