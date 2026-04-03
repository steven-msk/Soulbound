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

		// default numeric value computation:
		// A = base + Σ(flat)
		// B = A * (1 + Σ%) (% of A)
		// C = B * Π(multipliers)
		//
		// snapshot = C
		//
		//    where predicate(snapshot) is true:
		// P_flat = Σ(predicate flats)
		// P_%    = Σ(predicate %) (% of C)
		// P_mult = Π(predicate multipliers)
		//
		// final = C
		// final += P_flat
		// final *= (1 + P_%)
		// final *= P_mult
		//  
		// final = value_rule(final)
		public override float ComputeValue(float baseValue, IValueRule<float>? ruleOverride, IReadOnlyList<IAttributeModifier<float>> modifiers) {
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
			foreach (var modifier in numericModifiers) {
				float effective = modifier.GetNominalValue();

				// TODO: operation order will matter for targeting numeric modifiers
				foreach (var targeter in targeting) {
					IEnumerable<INumericModifier> resolved = targeter.GetTarget()!.Resolve(numericModifiers);

					if (resolved.Contains(modifier)) {
						targeter.Apply(ref effective);
					}
				}

				modifier.SetEffectiveValue(effective);
			}
			numericModifiers.RemoveAll(m => targeting.Contains(m));

			// filter predicates
			List<INumericModifier> predicate = numericModifiers
				.Where(m => m.HasPredicate())
				.ToList();
			numericModifiers.RemoveAll(m => m.HasPredicate());
			FilterOperations(numericModifiers,
				out List<INumericModifier> additive,
				out List<INumericModifier> additivePercent,
				out List<INumericModifier> multiplicative
			);

			// apply all flat adds/subtracts (A)
			float subtotalA = baseValue;
			foreach (var additiveMod in additive) {
				additiveMod.Apply(ref subtotalA);
			}

			// apply all percentage adds (B)
			float percentSum = 0f;
			foreach (var additivePercentMod in additivePercent) {
				additivePercentMod.Apply(ref percentSum);
			}
			float subtotalB = subtotalA * (1f + percentSum);

			// apply multipliers (C)
			float multplierProduct = 1f;
			foreach (var multiplicativeMod in multiplicative) {
				multiplicativeMod.Apply(ref multplierProduct);
			}
			float subtotalC = subtotalB * multplierProduct;

			FilterOperations(predicate,
				out List<INumericModifier> predicate_additive,
				out List<INumericModifier> predicate_additivePercent,
				out List<INumericModifier> predicate_multiplicative
			);

			IValueRule<float>? valueRule = ruleOverride ?? this.valueRule;
			AttributeSnapshot<float> snapshot = new(
				this, baseValue, subtotalC, modifiers, valueRule
			);
			float final = subtotalC;

			// apply predicate flat adds/subtracts (P_flat)
			foreach (var pred_additiveMod in predicate_additive) {
				if (pred_additiveMod.CheckPredicate(snapshot)) {
					pred_additiveMod.Apply(ref final);
				}
			}

			// apply predicate percentage adds/subtracts (P_%)
			float predicate_percentSum = 0f;
			foreach (var pred_additivePercentMod in predicate_additivePercent) {
				if (pred_additivePercentMod.CheckPredicate(snapshot)) {
					pred_additivePercentMod.Apply(ref predicate_percentSum);
				}
			}
			final *= 1f + predicate_percentSum;

			// apply predicate flat multipliers (P_multi)
			float predicate_multiplierProduct = 1f;
			foreach (var pred_multiplicativeMod in predicate_multiplicative) {
				if (pred_multiplicativeMod.CheckPredicate(snapshot)) {
					pred_multiplicativeMod.Apply(ref predicate_multiplierProduct);
				}
			}
			final *= predicate_multiplierProduct;

			// apply rule
			try {
				valueRule?.Apply(ref final);
			} catch (AttributeValueRuleViolationException e) {
				Logger.LogFatal(e);
				return baseValue;
			}

			return final;
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
