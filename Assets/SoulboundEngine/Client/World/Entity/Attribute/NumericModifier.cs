using System;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public class NumericModifier : INumericModifier {
		private readonly float amount;
		private readonly INumericOperation operation;
		private readonly Predicate<AttributeSnapshot<float>>? predicate;

		public NumericModifier(float amount, INumericOperation operation, Predicate<AttributeSnapshot<float>>? predicate = null) {
			this.amount = amount;
			this.operation = operation;
			this.predicate = predicate;
		}

		public void Apply(ref float value) {
			operation.Apply(ref value);
		}

		public bool CheckPredicate(AttributeSnapshot<float> snapshot) {
			return predicate?.Invoke(snapshot) ?? true;
		}

		public NumericOperationType GetOperationType() => operation.GetOperationType();

		public bool HasPredicate() => predicate != null;
	}
}
