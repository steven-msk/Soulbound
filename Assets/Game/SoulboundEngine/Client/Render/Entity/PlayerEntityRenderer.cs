using SoulboundEngine.World.Player;
using UnityEngine;

namespace SoulboundEngine.Client.Render.Entity {
	public sealed class PlayerEntityRenderer : EntityRenderer<PlayerEntity, PlayerRenderState, PlayerModel> {
		public PlayerEntityRenderer(FactoryContext context)
			: base(context) {
		}

		public override PlayerRenderState CreateRenderState(PlayerEntity entity) {
			return new PlayerRenderState {
				entity = entity,
				descriptor = PlayerEntity.DESCRIPTOR
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

	}
}
