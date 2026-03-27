using SoulboundBackend.Assets.Scripts.Client.UI.Button;
using SoulboundBackend.Core.Assets;
using SoulboundBackend.Core.Audio;

namespace SoulboundBackend.Client.UI {
	public sealed class UIAudioEventBank : AudioEventBank {
		private static readonly AssetKey click = new("click");

		public UIAudioEventBank() {
			this.AddListener<ButtonClickedEvent>(() => new AudioCue(click));
		}

	}
}
