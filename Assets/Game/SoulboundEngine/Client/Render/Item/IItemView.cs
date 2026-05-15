using UnityEngine;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.Render.Item {
	public interface IItemView {
		GameObject GetGameObject();
		VisualElement GetVisualElement();
		void SetVisible(bool visible);
		void Destroy();
		bool IsValid();

		public static IItemView Of(GameObject gameObject) {
			return new GameObjectImpl(gameObject);
		}

		public static IItemView Of(VisualElement visualElement) {
			return new VisualElementImpl(visualElement);
		}

		private sealed class GameObjectImpl : IItemView {
			private readonly GameObject gameObject;

			public GameObjectImpl(GameObject gameObject) {
				this.gameObject = gameObject;
			}

			void IItemView.Destroy() {
				if (this.gameObject != null) GameObject.Destroy(this.gameObject);
			}

			GameObject IItemView.GetGameObject() => this.gameObject;
			VisualElement IItemView.GetVisualElement() => null;
			bool IItemView.IsValid() => this.gameObject != null;

			void IItemView.SetVisible(bool visible) {
				if (this.gameObject != null) this.gameObject.SetActive(visible);
			}
		}

		private sealed class VisualElementImpl : IItemView {
			private readonly VisualElement visualElement;

			public VisualElementImpl(VisualElement visualElement) {
				this.visualElement = visualElement;
			}

			void IItemView.Destroy() => this.visualElement?.RemoveFromHierarchy();
			GameObject IItemView.GetGameObject() => null;
			VisualElement IItemView.GetVisualElement() => this.visualElement;
			bool IItemView.IsValid() => this.visualElement != null;

			void IItemView.SetVisible(bool visible) {
				if (this.visualElement != null) {
					this.visualElement.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
				}
			}
		}
	}
}
