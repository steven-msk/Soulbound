#nullable enable

namespace SoulboundBackend.Client.UI {
	public interface ISlotOperation {
		bool CanExecute();
		bool Execute();
	}
}
