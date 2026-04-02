using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Core.Registry;
using System;
using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public class NumericAttribute : AttributeType<float> {
		public NumericAttribute(Identifier identifier, IValueRule<float>? valueRule)
			: base(identifier, valueRule) {
		}

		// default numeric value computation:
		// A = base + Σ(flat)
		// B = A * (1 + Σ%)
		// C = B * Π(multipliers)
		//
		// snapshot = C
		//
		//    where predicate(snapshot) is true:
		// P_flat = Σ(predicate flats)
		// P_%    = Σ(predicate %)
		// P_mult = Π(predicate multipliers)
		//
		// final = C
		// final += P_flat
		// final *= (1 + P_%)
		// final *= P_mult
		//  
		// final = clamp(final)
		public override float ComputeValue(float baseValue, IValueRule<float>? ruleOverride, IReadOnlyList<IAttributeModifier<float>> modifiers) {
			List<INumericModifier> numericModifiers = new();
			foreach (var modifier in modifiers) {
				if (modifier is not INumericModifier numeric) {
					Logger.LogFatal(InvalidAttributeModifier(modifier.GetType().Name));
				} else {
					numericModifiers.Add(numeric);
				}
			}

			// filter operations
			List<INumericModifier> flatAddOrSubtract = new();
			List<INumericModifier> percentageAddOrSubtract = new();
			List<INumericModifier> multiplyOrDivide = new();
			List<INumericModifier> predicate = new();
			foreach (var modifier in numericModifiers) {
				if (modifier.HasPredicate()) predicate.Add(modifier);
				else if (modifier.HasFlatAddOrSubtractOperation()) flatAddOrSubtract.Add(modifier);
				else if (modifier.HasPercentageAddOrSubtractOperation()) percentageAddOrSubtract.Add(modifier);
				else if (modifier.HasFlatMultiplyOrDivideOperation()) multiplyOrDivide.Add(modifier);
			}

			// apply all flat adds/subtracts (A)
			float subtotalA = baseValue;
			foreach (var addOrSubtract in flatAddOrSubtract) {
				addOrSubtract.Apply(ref subtotalA);
			}

			// apply all percentage adds (B)
			float percentageSum = 0f;
			foreach (var addOrSubtract in percentageAddOrSubtract) {
				addOrSubtract.Apply(ref percentageSum);
			}
			float subtotalB = subtotalA * (1f + percentageSum);

			// apply multipliers (C)
			float multplierProduct = 1f;
			foreach (var multiOrDiv in multiplyOrDivide) {
				multiOrDiv.Apply(ref multplierProduct);
			}
			float subtotalC = subtotalB * multplierProduct;

			// TODO: create predicate modifiers

			// .........create snapshot.........
			// .......calculate predicate modifiers........

			float final = subtotalC;
			// .............apply predicate modifiers.................

			IValueRule<float>? valueRule = ruleOverride ?? this.valueRule;
			try {
				valueRule?.Apply(ref final);
			} catch (AttributeValueRuleViolationException e) {
				Logger.LogFatal(e);
				return baseValue;
			}

			return final;
		}

		private ArgumentException InvalidAttributeModifier(string received) {
			return new ArgumentException($"Expected modifier {typeof(INumericModifier).Name}, but received {received}");
		}
	}

}
