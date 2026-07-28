using SoulboundEngine.Client.UI.Screen;
using System;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI {
	public abstract class UXMLWidget : IDisposable {
		protected VisualElement root;
		private UXMLScreen screen;
		public string tooltip { get; private set; }
		public bool isVisible { get; private set; }
		public UXMLScreen Screen => this.screen ?? throw new NotSupportedException("UXML widget has not been added to a screen, but you are trying to access it.");

		public virtual void OnBind(VisualElement root) {
			this.root = root;
			this.root.style.display = this.isVisible ? DisplayStyle.Flex : DisplayStyle.None;
		}

		public virtual void Show() {
			if (this.isVisible) return;
			this.isVisible = true;

			this.root.style.display = DisplayStyle.Flex;
		}

		public virtual void Hide() {
			if (!this.isVisible) return;
			this.isVisible = false;

			this.root.style.display = DisplayStyle.None;
		}

		public virtual void Dispose() {
		}

		public void SetScreen(UXMLScreen screen) => this.screen = screen;

		public void SetTooltip(string tooltip) => this.tooltip = tooltip;
	}
}
