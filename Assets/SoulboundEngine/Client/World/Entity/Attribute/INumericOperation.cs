namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public interface INumericOperation {
		void Apply(ref float value);
		NumericOperationType GetOperationType();
	}
}
