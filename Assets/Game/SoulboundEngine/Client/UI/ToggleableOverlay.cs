namespace SoulboundEngine.Client.UI {
	public abstract class ToggleableOverlay<TNode> where TNode : UIOverlayNode {
		protected bool visible;
		protected TNode node;

		public virtual void Show() {
			if (this.visible) return;
			this.visible = true;
			this.CreateNodeIfNull();
			this.node.Show();
		}

		public virtual void Hide() {
			if (!this.visible) return;
			this.visible = false;
			this.node?.Hide();
		}

		public virtual void Toggle() {
			bool visible = !this.visible;

			if (visible) this.Show();
			else this.Hide();
		}

		protected void CreateNodeIfNull() {
			if (this.node != null) return;

			this.node = this.GetNode();
			this.node.onDestroy += () => {
				this.node = null;
				this.visible = false;
			};
			SoulboundClient.Instance.UIHandler.AddOverlay(this.node);
		}

		protected abstract TNode GetNode();

		public bool IsVisible() => this.visible;
	}
}
