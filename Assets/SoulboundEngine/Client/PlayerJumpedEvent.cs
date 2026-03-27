using SoulboundEngine.Client.Players;
using SoulboundEngine.Core.Event;

namespace SoulboundEngine.Client {
	public struct PlayerJumpedEvent : IGameEvent {
		public Player player;

		public PlayerJumpedEvent(Player player) {
			this.player = player;
		}
	}
}
