namespace SoulboundBackend.Client.UI {
	public sealed class NoSlotOperation : ISlotOperation {
		bool ISlotOperation.CanExecute() => true;
		bool ISlotOperation.Execute() => true;
	}
}
