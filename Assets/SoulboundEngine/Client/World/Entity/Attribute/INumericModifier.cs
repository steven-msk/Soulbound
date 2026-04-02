namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public interface INumericModifier : IAttributeModifier<float> {
		bool HasFlatAddOrSubtractOperation();
		bool HasPercentageAddOrSubtractOperation();
		bool HasFlatMultiplyOrDivideOperation();
	}
}
