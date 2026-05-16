using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI {
	public abstract class UxmlWidget {
		protected VisualElement root;
		public bool isVisible { get; private set; }

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
	}
}
