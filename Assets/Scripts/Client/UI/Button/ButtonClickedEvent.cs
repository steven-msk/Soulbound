using SoulboundBackend.Client.UI.Buttons;
using SoulboundBackend.Core.Event;

namespace SoulboundBackend.Assets.Scripts.Client.UI.Button {
	public readonly struct ButtonClickedEvent : IGameEvent {
		public readonly IButtonHandle buttonHandle;

		public ButtonClickedEvent(IButtonHandle buttonHandle) {
			this.buttonHandle = buttonHandle;
		}
	}
}
