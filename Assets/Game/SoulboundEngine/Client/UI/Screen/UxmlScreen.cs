using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	public abstract class UxmlScreen : Screen {
		private readonly VisualTreeAsset asset;

		protected UxmlScreen(VisualTreeAsset asset) {
			this.asset = asset;
		}

		protected sealed override void OnBuild(IScreenHandle handle) {
			TemplateContainer tree = this.asset.Instantiate();
			handle.Root.Add(tree);
			this.OnBind(handle.Root);
		}

		protected abstract void OnBind(VisualElement root);
	}
}
