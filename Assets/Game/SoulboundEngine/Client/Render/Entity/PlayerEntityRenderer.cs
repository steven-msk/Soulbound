using SoulboundEngine.Client.Players;
using UnityEngine;

namespace SoulboundEngine.Client.Render.Entity {
	public sealed class PlayerEntityRenderer : EntityRenderer<Player, PlayerRenderState, PlayerModel> {
		public override PlayerRenderState CreateRenderState(Player entity) {
			return new PlayerRenderState {
				entity = entity,
				descriptor = Player.DESCRIPTOR
			};
		}

		public override IEntityView CreateView(PlayerRenderState state, PlayerModel model) {
			GameObject obj = GameObject.Instantiate(model.prefab);
			PlayerTransform transform = obj.GetComponent<PlayerTransform>();
			transform.Init(state.entity);
			state.entity.SetPhysicsHandle(transform);
			state.entity.SetBoundingBoxHandle(transform);
			state.entity.SetTransformHandle(transform);
			return transform;
		}

		public override void DestroyView(IEntityView view) {
			view.Destroy();
		}

		public override void UpdateView(PlayerRenderState state, IEntityView view) {
		}
	}
}
