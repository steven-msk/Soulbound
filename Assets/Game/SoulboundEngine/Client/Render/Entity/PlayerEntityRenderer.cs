using SoulboundEngine.World.Entity;
using SoulboundEngine.World.Player;

namespace SoulboundEngine.Client.Render.Entity {
	public sealed class PlayerEntityRenderer : EntityRenderer<PlayerEntity, PlayerRenderState, PlayerModel> {
		public PlayerEntityRenderer(FactoryContext context)
			: base(context) {
		}

		public override PlayerRenderState CreateRenderState(PlayerEntity entity) {
			return new PlayerRenderState {
				entity = entity,
				descriptor = EntityType.PLAYER
			};
		}

		public override EntityViewHandle Create(PlayerRenderState state, PlayerModel model) {
			return EntityViewHandle.Instantiate(model.prefab);
		}

	}
}
