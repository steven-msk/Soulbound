using SoulboundBackend.Client.Players;
using SoulboundBackend.Core.Event;

namespace SoulboundBackend.Client {
	public struct PlayerJumpedEvent : IGameEvent {
		public Player player;

		public PlayerJumpedEvent(Player player) {
			this.player = player;
		}
	}
}
