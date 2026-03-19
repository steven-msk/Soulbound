namespace SoulboundBackend.Client.ItemSystem.Container {
	public interface ISlotOperation {
		bool CanExecute();
		bool Execute();
	}
}
