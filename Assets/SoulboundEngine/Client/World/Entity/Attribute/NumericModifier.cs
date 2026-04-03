using System;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public class NumericModifier : INumericModifier {
		private readonly float amount;
		private float effectiveAmount;
		private readonly INumericOperation operation;
		private readonly Predicate<IAttributeContext>? predicate;
		private readonly INumericModifierTarget? target;

		public NumericModifier(float amount, INumericOperation operation, INumericModifierTarget? target = null, Predicate<IAttributeContext>? predicate = null) {
			this.effectiveAmount = this.amount = amount;
			this.operation = operation;
			this.predicate = predicate;
			this.target = target;
		}

		public void Apply(ref float value) {
			operation.Apply(effectiveAmount, ref value);
		}

		public bool CheckPredicate(IAttributeContext context) {
			return predicate?.Invoke(context) ?? true;
		}
		public bool HasPredicate() => predicate != null;

		public NumericOperationType GetOperationType() => operation.GetOperationType();
		public INumericModifierTarget? GetTarget() => target;

		public void SetEffectiveValue(float effective) {
			this.effectiveAmount = effective;
		}
		public float GetNominalValue() => amount;
	}
}
