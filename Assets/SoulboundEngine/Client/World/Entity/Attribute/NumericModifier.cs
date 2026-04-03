using System;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public class NumericModifier : INumericModifier {
		private readonly float amount;
		private float effectiveAmount;
		private readonly INumericOperation operation;
		private readonly Predicate<AttributeSnapshot<float>>? predicate;
		private readonly INumericModifierTarget? target;

		public NumericModifier(float amount, INumericOperation operation, INumericModifierTarget? target = null, Predicate<AttributeSnapshot<float>>? predicate = null) {
			this.effectiveAmount = this.amount = amount;
			this.operation = operation;
			this.predicate = predicate;
			this.target = target;
		}

		public void Apply(ref float value) {
			operation.Apply(effectiveAmount, ref value);
		}

		public bool CheckPredicate(AttributeSnapshot<float> snapshot) {
			return predicate?.Invoke(snapshot) ?? true;
		}

		public float GetNominalValue() => amount;

		public NumericOperationType GetOperationType() => operation.GetOperationType();
		public INumericModifierTarget? GetTarget() => target;

		public bool HasPredicate() => predicate != null;

		public void SetEffectiveValue(float effective) {
			this.effectiveAmount = effective;
		}
	}
}
