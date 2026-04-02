using System;

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public class NumericModifier : IAttributeModifier<float> {
		protected readonly float amount;

		public NumericModifier(float amount) {
			this.amount = amount;
		}

		public void Apply(ref float value) {
			throw new NotImplementedException();
		}

		public bool HasPredicate() {
			throw new NotImplementedException();
		}
	}
}
