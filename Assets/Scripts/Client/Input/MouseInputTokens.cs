using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

namespace SoulboundBackend.Client.Input {
	public static partial class InputTokens {
		public static class Mouse {
			public static InputToken position;
			public static InputToken leftClick;
			public static InputToken rightClick;

			public static void Register(InputActionAsset asset) {
				position   = Create(asset, "Player/MousePosition");
				leftClick  = Create(asset, "Player/LeftClick");
				rightClick = Create(asset, "Player/RightClick");
			}
		}
	}
}
