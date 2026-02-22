using SoulboundBackend.Core;
using UnityEngine.InputSystem;

namespace SoulboundBackend.Client.Input {
	public static class InputTokens {
		public static readonly InputToken jump;
		public static readonly InputToken toggleDebugConsole;
		public static readonly InputToken enterCommand;

		static InputTokens() {
			InputActionAsset asset = Soulbound.inputActionAsset;

			jump = new InputToken(asset.FindAction("Player/Jump"));
			toggleDebugConsole = new InputToken(asset.FindAction("Debug/ToggleConsole"));
			enterCommand = new InputToken(asset.FindAction("Debug/EnterCommand"));
		}
	}
}
