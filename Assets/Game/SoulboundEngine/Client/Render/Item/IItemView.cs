using UnityEngine;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.Render.Item {
	public interface IItemView {
		void SetVisible(bool visible);
		void Destroy();
		bool IsValid();
		void SetPosition(Vector2 pos);

		public static IItemView Of(GameObject gameObject) {
			return new GameObjectImpl(gameObject);
		}

		public static IItemView Of(VisualElement visualElement) {
			return new VisualElementImpl(visualElement);
		}

		public sealed class GameObjectImpl : IItemView {
			private readonly GameObject gameObject;

			public GameObjectImpl(GameObject gameObject) {
				this.gameObject = gameObject;
			}

			public void Destroy() {
				if (this.gameObject != null) GameObject.Destroy(this.gameObject);
			}

			bool IItemView.IsValid() => this.gameObject != null;

			public void SetVisible(bool visible) {
				if (this.gameObject != null) this.gameObject.SetActive(visible);
			}

			void IItemView.SetPosition(Vector2 pos) {
				this.gameObject.transform.position = pos;
			}

			public GameObject GetGameObject() => this.gameObject;
		}

		public sealed class VisualElementImpl : IItemView {
			private readonly VisualElement visualElement;

			public VisualElementImpl(VisualElement visualElement) {
				this.visualElement = visualElement;
			}

			void IItemView.Destroy() => this.visualElement?.RemoveFromHierarchy();

			bool IItemView.IsValid() => this.visualElement != null;

			void IItemView.SetVisible(bool visible) {
				if (this.visualElement != null) {
					this.visualElement.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
				}
			}

			void IItemView.SetPosition(Vector2 pos) {
				if (this.visualElement != null) {
					this.visualElement.style.translate = pos;
				}
			}
		}
	}
}
