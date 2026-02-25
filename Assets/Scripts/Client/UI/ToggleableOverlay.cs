using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.UI {
	public abstract class ToggleableOverlay<TNode> where TNode : UIOverlayNode {
		protected bool visible;
		protected TNode node;

		public virtual void Toggle() {
			visible = !visible;
			CreateNodeIfNull();

			if (!visible) node.Hide();
			else node.Show();
		}

		protected void CreateNodeIfNull() {
			if (node != null) return;

			node = GetNode();
			node.onDestroy += () => {
				node = null;
				visible = false;
			};
			Soulbound.instance.GetUIHandler().AddOverlay(node);
		}

		protected abstract TNode GetNode();

		public bool IsVisible() => visible;
	}
}
