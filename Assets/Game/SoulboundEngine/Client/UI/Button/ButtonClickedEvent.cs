using SoulboundEngine.Client.UI.Buttons;
using SoulboundEngine.Core.Event;

namespace SoulboundEngine.Assets.Scripts.Client.UI.Button {
	public readonly struct ButtonClickedEvent : IGameEvent {
		public readonly IButtonHandle buttonHandle;

		public ButtonClickedEvent(IButtonHandle buttonHandle) {
			this.buttonHandle = buttonHandle;
		}
	}
}
