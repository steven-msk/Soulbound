using SoulboundEngine.Core.Event;

namespace SoulboundEngine.Client {
	public struct PlayerJumpedEvent : IGameEvent {
		public Player.Player player;

		public PlayerJumpedEvent(Player.Player player) {
			this.player = player;
		}
	}
}
