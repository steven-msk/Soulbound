using SoulboundEngine.Core.Event;
using SoulboundEngine.World.Player;

namespace SoulboundEngine.Client {
	public struct PlayerJumpedEvent : IGameEvent {
		public PlayerEntity player;

		public PlayerJumpedEvent(PlayerEntity player) {
			this.player = player;
		}
	}
}
