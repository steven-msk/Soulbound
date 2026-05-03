using UnityEngine;

namespace SoulboundEngine.Client.Render.Item {
	public interface IItemView {
		GameObject GetGameObject();
		void SetVisible(bool visible);
		void Destroy();

		public static IItemView Of(GameObject gameObject) {
			return new Impl(gameObject);
		}

		private sealed class Impl : IItemView {
			private readonly GameObject gameObject;

			public Impl(GameObject gameObject) {
				this.gameObject = gameObject;
			}

			void IItemView.Destroy() => GameObject.Destroy(this.gameObject);
			GameObject IItemView.GetGameObject() => this.gameObject;
			void IItemView.SetVisible(bool visible) => this.gameObject.SetActive(visible);
		}
	}
}
