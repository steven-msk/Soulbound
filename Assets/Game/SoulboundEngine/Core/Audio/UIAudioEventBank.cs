using SoulboundEngine.Assets.Scripts.Client.UI.Button;
using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.Audio;

namespace SoulboundEngine.Client.UI {
	public sealed class UIAudioEventBank : AudioEventBank {
		private static readonly AssetKey click = new("click");

		public UIAudioEventBank() {
			this.AddListener<ButtonClickedEvent>(() => new AudioCue(click));
		}

	}
}
