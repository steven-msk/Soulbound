using SoulboundEngine.Client.UI.Screen;
using System;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI {
	public abstract class UXMLWidget : IDisposable {
		protected VisualElement root;
		private UXMLScreen screen;
		public string tooltip { get; private set; }
		public bool isVisible { get; protected set; }
		public UXMLScreen Screen => this.screen ?? throw new NotSupportedException("UXML widget has not been added to a screen, but you are trying to access it.");

		public virtual void OnBind(VisualElement root) {
			this.root = root;
			root.style.visibility = this.isVisible ? Visibility.Visible : Visibility.Hidden;
		}

		public virtual void Show() {
			this.isVisible = true;
			this.root.style.visibility = Visibility.Visible;
		}

		public virtual void Hide() {
			this.isVisible = false;
			this.root.style.visibility = Visibility.Hidden;
		}

		public virtual void Dispose() {
		}

		public void SetScreen(UXMLScreen screen) => this.screen = screen;

		public void SetTooltip(string tooltip) => this.tooltip = tooltip;
	}
}
