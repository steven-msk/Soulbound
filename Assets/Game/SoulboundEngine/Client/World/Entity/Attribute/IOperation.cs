namespace SoulboundEngine.Client.World.Entity.Attribute {
	public interface IOperation {
		void Apply(double effectiveAmount, ref double targetValue);
		OperationType GetOperationType();
	}
}
