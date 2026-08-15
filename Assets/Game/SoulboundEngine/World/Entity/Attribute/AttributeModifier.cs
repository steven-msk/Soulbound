using SoulboundEngine.Registry;

#nullable enable

namespace SoulboundEngine.World.Entity.Attribute {
	public record AttributeModifier(Identifier identifier, double value, IOperation operation, IModifierTarget? target) {
		public void Apply(double? effectiveOverride, ref double targetValue) {
			this.operation.Apply(effectiveOverride ?? this.value, ref targetValue);
		}

		public OperationType GetOperationType() => this.operation.GetOperationType();

		public bool IdMatches(Identifier identifier) => this.identifier.Equals(identifier);
	}
}
