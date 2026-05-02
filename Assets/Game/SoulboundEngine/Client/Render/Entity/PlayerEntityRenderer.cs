using SoulboundEngine.Client.Players;
using SoulboundEngine.Core.Assets;
using System;
using UnityEngine;

namespace SoulboundEngine.Client.Render.Entity {
	public sealed class PlayerEntityRenderer : EntityRenderer<Player, PlayerRenderState> {
		public override PlayerRenderState CreateRenderState(Player entity) {
			return new PlayerRenderState {
				entity = entity,
				descriptor = Player.DESCRIPTOR
			};
		}

		public override IEntityView CreateView(PlayerRenderState state) {
			GameObject obj = GameObject.Instantiate(AssetManager.Resolve<GameObject>(new AssetKey("player")));
			PlayerTransform transform = obj.GetComponent<PlayerTransform>();
			return transform;
		}

		public override void DestroyView(IEntityView view) {
			view.Destroy();
		}

		public override void UpdateView(PlayerRenderState state, IEntityView view) {
			throw new NotImplementedException();
		}
	}
}
