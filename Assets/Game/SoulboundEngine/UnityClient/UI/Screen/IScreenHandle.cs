using UnityEngine.UIElements;

namespace SoulboundEngine.UnityClient.UI.Screen {
	public interface IScreenHandle {
		VisualElement Root { get; }

		void Show();
		void Hide();
		void Dispose();
		Screen GetScreen();
	}
}
