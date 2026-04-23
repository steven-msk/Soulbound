using SoulboundEngine.Core.Assets;

namespace SoulboundEngine.Core.Audio {
	public readonly struct AudioCue {
		public readonly AssetKey assetKey;
		public readonly float? volumeOverride;

		public AudioCue(AssetKey assetKey) : this() {
			this.assetKey = assetKey;
		}

		public AudioCue(AssetKey assetKey, float volumeOverride) {
			this.assetKey = assetKey;
			this.volumeOverride = volumeOverride;
		}
	}
}
