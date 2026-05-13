using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	public sealed class UIToolkitScreenHandle : IScreenHandle {
		public VisualElement Root { get; }
		private readonly Screen screen;

		public UIToolkitScreenHandle(Screen screen, VisualElement root) {
			this.screen = screen;
			this.Root = root;
		}

		public void Dispose() {
			this.screen.OnDispose(this);
			this.Root.RemoveFromHierarchy();
		}

		public void Hide() {
			this.screen.OnHide(this);
			this.Root.style.display = DisplayStyle.None;
		}

		public void Show() {
			this.screen.OnShow(this);
			this.Root.style.display = DisplayStyle.Flex;
		}

		public Screen GetScreen() => this.screen;
	}
}
