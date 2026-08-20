
namespace SoulboundEngine.Client.Render.Item {
	using SoulboundEngine.Client.Util;
	using SoulboundEngine.Common.Math;
	using UnityEngine;
	using UnityEngine.UIElements;

#nullable enable

	public abstract record ItemViewHandle {
		public abstract void SetVisible(bool visible);

		public abstract bool IsValid();

		public abstract void SetPosition(Vec2d position);

		public static GameObjectBacked Of(GameObject gameObject) {
			return new GameObjectBacked(gameObject);
		}

		public static VisualElementBacked Of(VisualElement visualElement) {
			return new VisualElementBacked(visualElement);
		}

		public sealed record GameObjectBacked(GameObject gameObject) : ItemViewHandle {
			public override bool IsValid() {
				return this.gameObject;
			}

			public override void SetPosition(Vec2d position) {
				this.gameObject.transform.position = position.ToVector2();
			}

			public override void SetVisible(bool visible) {
				this.gameObject.SetActive(visible);
			}
		}

		public sealed record VisualElementBacked(VisualElement visualElement) : ItemViewHandle {
			public override bool IsValid() {
				return this.visualElement != null;
			}

			public override void SetPosition(Vec2d position) {
				this.visualElement.style.translate = position.ToVector2();
			}

			public override void SetVisible(bool visible) {
				this.visualElement.style.visibility = visible ? Visibility.Visible : Visibility.Hidden;
			}
		}
	}
}
