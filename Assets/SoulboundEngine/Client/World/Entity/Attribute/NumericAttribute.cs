using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Core.Registry;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public class NumericAttribute : AttributeType<float> {
		public NumericAttribute(Identifier identifier, IValueRule<float>? valueRule)
			: base(identifier, valueRule) {
		}

		public override float ComputeValue(float baseValue, IValueRule<float>? ruleOverride, IReadOnlyList<IAttributeModifier<float>> modifiers, IAttributeContext attributeContext) {
			List<INumericModifier> numericModifiers = new();
			foreach (var modifier in modifiers) {
				if (modifier is not INumericModifier numeric) {
					Logger.LogFatal(InvalidAttributeModifier(modifier.GetType().Name));
				} else {
					numericModifiers.Add(numeric);
				}
			}

			// filter targeting modifiers
			List<INumericModifier> targeting = numericModifiers
				.Where(m => m.GetTarget() != null)
				.ToList();
			Dictionary<INumericModifier, List<INumericModifier>> modifierToItsTargeters = new();
			foreach (var targeter in targeting) {
				IEnumerable<INumericModifier> targets = targeter.GetTarget()!.Resolve(numericModifiers);

				foreach (var target in targets) {
					if (!modifierToItsTargeters.ContainsKey(target)) {
						modifierToItsTargeters[target] = new List<INumericModifier>();
					}
					modifierToItsTargeters[target].Add(targeter);
				}
			}

			foreach (var target in modifierToItsTargeters.Keys) {
				List<INumericModifier> targeters = modifierToItsTargeters[target]
					.Where(m => !m.HasPredicate() || m.CheckPredicate(attributeContext))
					.ToList();

				// TODO: filter predicate targeters

				float effective = CalculateModifiers(target.GetNominalValue(), targeters);

				target.SetEffectiveValue(effective);
			}

			numericModifiers.RemoveAll(m => targeting.Contains(m));

			// filter predicates
			List<INumericModifier> predicateModifiers = numericModifiers
				.Where(m => m.HasPredicate())
				.ToList();
			numericModifiers.RemoveAll(m => m.HasPredicate());
			predicateModifiers.RemoveAll(m => !m.CheckPredicate(attributeContext));

			float prePredicateResult = CalculateModifiers(baseValue, numericModifiers);
			float final = CalculateModifiers(prePredicateResult, predicateModifiers);

			// apply rule
			try {
				IValueRule<float>? valueRule = ruleOverride ?? this.valueRule;
				valueRule?.Apply(ref final);
			} catch (AttributeValueRuleViolationException e) {
				Logger.LogFatal(e);
				return baseValue;
			}

			return final;
		}

		// default numeric value computation:
		// A = base + Σ(flat)
		// B = A * (1 + Σ%) (% of A)
		// C = B * Π(multipliers)
		private float CalculateModifiers(float baseValue, IEnumerable<INumericModifier> modifiers) {
			FilterOperations(modifiers.ToList(),
				out List<INumericModifier> additive,
				out List<INumericModifier> additivePercent,
				out List<INumericModifier> multiplicative
			);

			// apply all flat adds/subtracts (A)
			float subtotalA = baseValue;
			foreach (var modifier in additive) {
				modifier.Apply(ref subtotalA);
			}

			// apply all percentage adds (B)
			float percentSum = 0f;
			foreach (var modifier in additivePercent) {
				modifier.Apply(ref percentSum);
			}
			float subtotalB = subtotalA * (1f + percentSum);

			// apply multipliers (C)
			float multplierProduct = 1f;
			foreach (var modifier in multiplicative) {
				modifier.Apply(ref multplierProduct);
			}
			float subtotalC = subtotalB * multplierProduct;

			return subtotalC;
		}

		private void FilterOperations(
			List<INumericModifier> modifiers,
			out List<INumericModifier> additive,
			out List<INumericModifier> additivePercent,
			out List<INumericModifier> multiplicative
		) {
			additive = new List<INumericModifier>();
			additivePercent = new List<INumericModifier>();
			multiplicative = new List<INumericModifier>();
			foreach (var modifier in modifiers) {
				NumericOperationType opType = modifier.GetOperationType();

				if (opType == NumericOperationType.Additive) additive.Add(modifier);
				else if (opType == NumericOperationType.AdditivePercent) additivePercent.Add(modifier);
				else if (opType == NumericOperationType.Multiplicative) multiplicative.Add(modifier);
			}
		}

		private ArgumentException InvalidAttributeModifier(string received) {
			return new ArgumentException($"Expected modifier {typeof(INumericModifier).Name}, but received {received}");
		}
	}

}
