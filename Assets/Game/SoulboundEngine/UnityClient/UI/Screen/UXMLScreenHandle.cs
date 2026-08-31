using UnityEngine.UIElements;

namespace SoulboundEngine.UnityClient.UI.Screen {
	public sealed class UXMLScreenHandle : IScreenHandle {
		public VisualElement Root { get; }
		private readonly Screen screen;
		private bool isVisible;

		public UXMLScreenHandle(Screen screen, VisualElement root) {
			this.screen = screen;
			this.Root = root;
		}

		public void Dispose() {
			this.screen.OnDispose(this);
			this.Root.RemoveFromHierarchy();
			this.isVisible = false;
		}

		public void Hide() {
			if (!this.isVisible) return;

			this.screen.OnHide(this);
			this.Root.style.display = DisplayStyle.None;
			this.isVisible = false;
		}

		public void Show() {
			if (this.isVisible) return;

			this.screen.OnShow(this);
			this.Root.style.display = DisplayStyle.Flex;
			this.isVisible = true;
		}

		public Screen GetScreen() => this.screen;

	}
}
