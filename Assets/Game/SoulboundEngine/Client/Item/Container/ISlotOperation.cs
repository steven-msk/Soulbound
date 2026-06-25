namespace SoulboundEngine.Client.Item.Container {
	public interface ISlotOperation {
		bool CanExecute();
		bool Execute();
	}
}
