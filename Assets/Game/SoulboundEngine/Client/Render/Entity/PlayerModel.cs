using UnityEngine;

namespace SoulboundEngine.Client.Render.Entity {
	public sealed class PlayerModel : EntityModel {
		public readonly GameObject prefab;

		public PlayerModel(GameObject prefab) {
			this.prefab = prefab;
		}
	}
}
