namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public interface IOperation {
		void Apply(double effectiveAmount, ref double targetValue);
		OperationType GetOperationType();
	}
}
