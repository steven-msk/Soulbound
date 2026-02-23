using SoulboundBackend.Common;
using SoulboundBackend.Core;
using System;
using UnityEngine.InputSystem;

namespace SoulboundBackend.Client.Input {
	public static partial class InputTokens {
		public static InputToken toggleDebugConsole;
		public static InputToken enterCommand;

		public static InputToken esc;
		public static InputToken mousePosition;
		public static InputToken leftClick;
		public static InputToken rightClick;

		public static void Register(InputActionAsset asset) {
			toggleDebugConsole = Create(asset, "Debug/ToggleConsole");
			enterCommand	   = Create(asset, "Debug/EnterCommand");

			esc				   = Create(asset, "Player/Esc");
			mousePosition      = Create(asset, "Player/MousePosition");
			leftClick		   = Create(asset, "Player/LeftClick");
			rightClick		   = Create(asset, "Player/RightClick");

			Player.Register(asset);
		}

		internal static InputToken Create(InputActionAsset asset, string path) {
			return new InputToken(asset.FindAction(path));
		}
	}
}
