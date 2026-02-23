using SoulboundBackend.Common;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Debug.Logging;
using System;
using UnityEngine.InputSystem;

namespace SoulboundBackend.Client.Input {
	public static partial class InputTokens {
		public static void Register(InputActionAsset asset) {
			Keyboard.Register(asset);
			Debug.Register(asset);
			Mouse.Register(asset);
			Player.Register(asset);
		}

		internal static InputToken Create(InputActionAsset asset, string path) {
			InputAction action = asset.FindAction(path, true);
			return new InputToken(action);
		}
	}
}
