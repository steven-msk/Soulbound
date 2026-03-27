namespace SoulboundEngine.Client.ItemSystem.Container {
	public interface ISlotOperation {
		bool CanExecute();
		bool Execute();
	}
}
