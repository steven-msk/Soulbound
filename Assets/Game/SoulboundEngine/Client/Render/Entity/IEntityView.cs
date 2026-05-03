using UnityEngine;

namespace SoulboundEngine.Client.Render.Entity {
	public interface IEntityView {
		GameObject GetGameObject();
		void SetVisible(bool visible);
		void Destroy();

		public static IEntityView Of(GameObject gameObject) => new Impl(gameObject);

		private sealed class Impl : IEntityView {
			private readonly GameObject gameObject;

			public Impl(GameObject gameObject) {
				this.gameObject = gameObject;
			}

			public void Destroy() => GameObject.Destroy(this.gameObject);

			public GameObject GetGameObject() => this.gameObject;

			public void SetVisible(bool visible) => this.gameObject.SetActive(visible);
		}
	}
}
