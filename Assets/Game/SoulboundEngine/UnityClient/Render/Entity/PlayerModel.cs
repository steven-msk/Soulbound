using UnityEngine;

namespace SoulboundEngine.UnityClient.Render.Entity {
	public sealed class PlayerModel : EntityModel {
		public readonly GameObject prefab;

		public PlayerModel(GameObject prefab) {
			this.prefab = prefab;
		}
	}
}
