namespace SoulboundEngine.Client.UI.Screen {
	public interface IScreenHandle {
		void Show();
		void Hide();
		void Dispose();
		Screen GetScreen();
	}
}
