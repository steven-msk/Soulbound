using System;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	public sealed class UIToolkitScreenHandle : IScreenHandle {
		public VisualElement Root { get; }
		private readonly Screen screen;
		private bool isVisible;

		public UIToolkitScreenHandle(Screen screen, VisualElement root) {
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

		[Obsolete]
		public void AddOverlay(VisualElement element) {
			this.Root.Add(element);
		}
	}
}
