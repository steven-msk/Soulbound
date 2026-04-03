#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public interface INumericModifier : IAttributeModifier<float> {
		NumericOperationType GetOperationType();
		INumericModifierTarget? GetTarget();
		float GetNominalValue();
		void SetEffectiveValue(float effectiveValue);
	}
}
