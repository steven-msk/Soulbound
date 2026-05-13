using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	public sealed class UIToolkitScreenHandle : IScreenHandle {
		private readonly VisualElement root;
		private readonly Screen screen;

		public UIToolkitScreenHandle(Screen screen, VisualElement root) {
			this.screen = screen;
			this.root = root;
		}

		public void Dispose() {
			this.screen.OnDispose(this);
			this.root.RemoveFromHierarchy();
		}

		public void Hide() {
			this.screen.OnHide(this);
			this.root.style.display = DisplayStyle.None;
		}

		public void Show() {
			this.screen.OnShow(this);
			this.root.style.display = DisplayStyle.Flex;
		}

		public Screen GetScreen() => this.screen;
	}
}
