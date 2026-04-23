using SoulboundEngine.Client;
using SoulboundEngine.Core.Assets;

namespace SoulboundEngine.Core.Audio {
	public class WorldAudioEventBank : AudioEventBank {
		private static readonly AssetKey blockBreak = new("blockBreak");
		private static readonly AssetKey blockPlace = new("blockPlace");
		private static readonly AssetKey jump = new("jump");

		public WorldAudioEventBank() {
			this.AddListener<BlockBrokenEvent>(() => new AudioCue(blockBreak));
			this.AddListener<BlockPlacedEvent>(() => new AudioCue(blockPlace));
			this.AddListener<PlayerJumpedEvent>(() => new AudioCue(jump));
		}

	}
}
