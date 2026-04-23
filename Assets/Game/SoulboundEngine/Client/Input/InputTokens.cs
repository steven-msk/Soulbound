using SoulboundEngine.Common;
using SoulboundEngine.Core;
using SoulboundEngine.Client.Debug.Logging;
using System;
using UnityEngine.InputSystem;

namespace SoulboundEngine.Client.Input {
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
