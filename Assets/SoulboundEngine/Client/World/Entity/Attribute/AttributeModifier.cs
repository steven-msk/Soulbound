using SoulboundEngine.Core.Registry;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public record AttributeModifier(Identifier identifier, double value, IOperation operation, IModifierTarget? target) {
		public void Apply(double? effectiveOverride, ref double targetValue) {
			operation.Apply(effectiveOverride ?? this.value, ref targetValue);
		}

		public OperationType GetOperationType() => operation.GetOperationType();

		public bool IdMatches(Identifier identifier) => this.identifier.Equals(identifier);
	}
}
