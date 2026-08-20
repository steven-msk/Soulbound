using SoulboundEngine.World.Player;

namespace SoulboundEngine.Event {
	public struct PlayerJumpedEvent : IGameEvent {
		public PlayerEntity player;

		public PlayerJumpedEvent(PlayerEntity player) {
			this.player = player;
		}
	}
}
