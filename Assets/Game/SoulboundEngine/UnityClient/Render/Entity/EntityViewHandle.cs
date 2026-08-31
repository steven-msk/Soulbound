namespace SoulboundEngine.UnityClient.Render.Entity {
	using SoulboundEngine.UnityClient.Util;
	using SoulboundEngine.Common.Math;
	using UnityEngine;

	public readonly struct EntityViewHandle {
		private readonly GameObject gameObject;

		private EntityViewHandle(GameObject gameObject) {
			this.gameObject = gameObject;
		}

		public static EntityViewHandle Of(GameObject gameObject) {
			return new EntityViewHandle(gameObject);
		}

		public static EntityViewHandle Instantiate(GameObject prefab) {
			return new EntityViewHandle(GameObject.Instantiate(prefab));
		}

		public GameObject GetGameObject() => this.gameObject;

		public T GetComponent<T>() => this.gameObject.GetComponent<T>();

		public bool IsValid() => this.gameObject;

		public void SetVisible(bool visible) {
			this.gameObject.SetActive(visible);
		}

		public void SetPosition(Vec2d pos) {
			this.GetComponent<Transform>().position = pos.ToVector2();
		}
	}
}
