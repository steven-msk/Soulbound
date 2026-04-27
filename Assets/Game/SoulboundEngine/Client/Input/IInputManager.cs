namespace SoulboundEngine.Client.Input {
	public interface IInputManager {
		void AddListener(InputEventListener listener);
		void RemoveListener(InputEventListener listener);

		void AddHandler(IInputEventHandler handler);
		void RemoveHandler(IInputEventHandler handler);
	}
}
