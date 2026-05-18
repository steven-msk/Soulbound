using System;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	public interface IScreenHandle {
		VisualElement Root { get; }

		void Show();
		void Hide();
		void Dispose();
		Screen GetScreen();

		[Obsolete]
		void AddOverlay(VisualElement element);
	}
}
