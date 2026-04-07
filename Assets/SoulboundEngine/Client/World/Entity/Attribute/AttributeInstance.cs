using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Common;
using SoulboundEngine.Core.Registry;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public sealed class AttributeInstance {
		private readonly Dictionary<Identifier, AttributeModifier> idToModifier = new();
		private readonly Dictionary<Identifier, AttributeModifier> persistentModifiers = new();
		private readonly Dictionary<Identifier, Func<bool>> idToPredicate = new();
		private readonly Dictionary<Identifier, bool> lastPredicateState = new();
		private readonly RegistryEntry<EntityAttribute> type;
		private readonly IValueRule? ruleOverride;
		private double value;
		private bool dirty;

		public   AttributeInstance(RegistryEntry<EntityAttribute> type, IValueRule? ruleOverride = null) {
			this.type = type;
			this.dirty = true;
			this.ruleOverride = ruleOverride;
		}

		public double baseValue { get; set; }

		private void AddModifier(AttributeModifier modifier) {
			idToModifier.Add(modifier.identifier, modifier);
			dirty = true;
		}

		public void AddPersistentModifier(AttributeModifier modifier) {
			AddModifier(modifier);
			persistentModifiers.Add(modifier.identifier, modifier);
		}
		public void AddPredicateModifier(AttributeModifier modifier, Func<bool> predicate) {
			AddPersistentModifier(modifier);
			idToPredicate.Add(modifier.identifier, predicate);
		}

		public void AddPersistentModifiers(params AttributeModifier[] modifiers) {
			foreach (var modifier in modifiers) {
				AddPersistentModifier(modifier);
			}
		}
		public void AddPredicateModifiers(params (AttributeModifier modifier, Func<bool> predicate)[] modifiers) {
			foreach (var (modifier, predicate) in modifiers) {
				AddPredicateModifier(modifier, predicate);
			}
		}

		public void ClearModifiers() {
			idToModifier.Clear();
			persistentModifiers.Clear();
			idToPredicate.Clear();
			dirty = true;
		}

		public void OnUpdate() {
			if (HasPredicateModifiers()) {
				bool anyChanged = false;

				foreach (var id in idToPredicate.Keys) {
					if (anyChanged) break;

					bool current = idToPredicate[id]();
					if (current != lastPredicateState.GetValueOrDefault(id, !current)) {
						anyChanged = true;
					}
					lastPredicateState[id] = current;
				}

				if (anyChanged) dirty = true;
			}
		}

		private double ComputeValue() {
			if (!dirty) return value;

			List<AttributeModifier> allModifiers = GetModifiers().ToList();

			// filter targeting modifiers
			ISet<AttributeModifier> targeting = GetTargetingModifiers(allModifiers);
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
				HashSet<AttributeModifier> predicate_targeters = GetPredicateModifiers(targeters).ToHashSet();
				targeters.RemoveAll(m => predicate_targeters.Contains(m));

				double effectiveOverride = CalculateModifiedValue(target.value, targeters, Dictionaries.Empty<AttributeModifier, double>());

				predicate_targeters.RemoveWhere(m => !idToPredicate[m.identifier]());
				effectiveOverride = CalculateModifiedValue(effectiveOverride, predicate_targeters, Dictionaries.Empty<AttributeModifier, double>());

				effectiveOverrides.Add(target, effectiveOverride);
			}

			// filter predicates
			HashSet<AttributeModifier> predicateModifiers = GetPredicateModifiers(allModifiers).ToHashSet();
			allModifiers.RemoveAll(m => predicateModifiers.Contains(m));
			double prePredicateResult = CalculateModifiedValue(this.baseValue, allModifiers, effectiveOverrides);

			predicateModifiers.RemoveWhere(m => !idToPredicate[m.identifier]());
			double final = CalculateModifiedValue(prePredicateResult, predicateModifiers, effectiveOverrides);

			// apply value rule
			try {
				IValueRule? valueRule = ruleOverride ?? type.GetValue().ValueRule;
				valueRule?.Apply(ref final);
			} catch (AttributeValueRuleViolationException e) {
				Logger.LogFatal(e);
				final = baseValue;
			} finally {
				value = final;
			}

			dirty = false;
			return value;
		}

		// default numeric value computation:
		// A = base + Σ(flat)
		// B = A * (1 + Σ%) (% of A)
		// C = B * Π(multipliers)
		private double CalculateModifiedValue(double baseValue, IEnumerable<AttributeModifier> modifiers, Dictionary<AttributeModifier, double> effectiveOverrides) {
			FilterOperations(modifiers,
				out List<AttributeModifier> additive,
				out List<AttributeModifier> additivePercent,
				out List<AttributeModifier> multiplicative
			);
			double? GetOverride(AttributeModifier modifier) {
				return effectiveOverrides.TryGetValue(modifier, out double value)
					? value
					: null;
			}

			// apply all flat adds/subtracts (A)
			double A = baseValue;
			foreach (var modifier in additive) {
				modifier.Apply(GetOverride(modifier), ref A);
			}

			// apply all percentage adds (B)
			double percentSum = 0d;
			foreach (var modifier in additivePercent) {
				modifier.Apply(GetOverride(modifier), ref percentSum);
			}
			double B = A * (1d + percentSum);

			// apply multipliers (C)
			double multiplierProduct = 1d;
			foreach (var modifier in multiplicative) {
				modifier.Apply(GetOverride(modifier), ref multiplierProduct);
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
			if (dirty) ComputeValue();
			return value;
		}

		public IValueRule? GetValueRuleOverride() => ruleOverride;

		private bool IsPredicate(AttributeModifier modifier) => idToPredicate.ContainsKey(modifier.identifier);

		public RegistryEntry<EntityAttribute> GetAttribute() => type;

		public bool TryGetModifier(Identifier identifier, out AttributeModifier modifier) {
			return idToModifier.TryGetValue(identifier, out modifier);
		}

		public ISet<AttributeModifier> GetModifiers() => idToModifier.Values.ToHashSet();
		public IReadOnlyDictionary<Identifier, AttributeModifier> GetModifiersByOperation(IOperation operation) {
			return idToModifier
				.Where(kvp => kvp.Value.operation.Equals(operation))
				.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
		}
		public bool HasModifier(Identifier identifier) => idToModifier.ContainsKey(identifier);
		public ISet<AttributeModifier> GetPersistentModifiers() => persistentModifiers.Values.ToHashSet();
		private ISet<AttributeModifier> GetTargetingModifiers(IEnumerable<AttributeModifier> modifiers) {
			return modifiers
				.Where(m => m.target != null)
				.ToHashSet();
		}
		private ISet<AttributeModifier> GetPredicateModifiers(IEnumerable<AttributeModifier> modifiers) {
			return modifiers
				.Where(m => IsPredicate(m))
				.ToHashSet();
		}
		public bool HasPredicateModifiers() => idToPredicate.Any();

		public void OverwritePersistentModifier(AttributeModifier modifier) {
			if (!idToModifier.ContainsKey(modifier.identifier)) return;

			idToModifier[modifier.identifier] = modifier;
			persistentModifiers[modifier.identifier] = modifier;

			dirty = true;
		}

		public void RemoveModifier(AttributeModifier modifier) => RemoveModifier(modifier.identifier);
		public void RemoveModifier(Identifier identifier) {
			idToModifier.Remove(identifier);
			persistentModifiers.Remove(identifier);
			idToPredicate.Remove(identifier);
			dirty = true;
		}

	}
}
