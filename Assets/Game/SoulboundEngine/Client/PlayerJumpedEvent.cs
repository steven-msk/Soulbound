using SoulboundEngine.Core.Event;

namespace SoulboundEngine.Client {
	public struct PlayerJumpedEvent : IGameEvent {
		public Player.PlayerEntity player;

		public PlayerJumpedEvent(Player.PlayerEntity player) {
			this.player = player;
		}
	}
}
