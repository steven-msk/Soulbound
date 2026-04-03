namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public interface INumericOperation {
		void Apply(float effectiveAmount, ref float targetValue);
		NumericOperationType GetOperationType();
	}
}
