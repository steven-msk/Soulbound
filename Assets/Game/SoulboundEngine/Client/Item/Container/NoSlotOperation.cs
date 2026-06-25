namespace SoulboundEngine.Client.Item.Container {
	public sealed class NoSlotOperation : ISlotOperation {
		bool ISlotOperation.CanExecute() => true;
		bool ISlotOperation.Execute() => true;
	}
}
