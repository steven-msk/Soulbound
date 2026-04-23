using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

namespace SoulboundEngine.Client.Input {
	public static partial class InputTokens {
		public static class Mouse {
			public static InputToken position;
			public static InputToken leftClick;
			public static InputToken rightClick;
			public static InputToken middleClick;
			public static InputToken forward;
			public static InputToken backward;

			public static void Register(InputActionAsset asset) {
				position    = Create(asset, "Mouse/Position");
				leftClick   = Create(asset, "Mouse/LeftClick");
				rightClick  = Create(asset, "Mouse/RightClick");
				middleClick = Create(asset, "Mouse/MiddleClick");
				forward		= Create(asset, "Mouse/Forward");
				backward	= Create(asset, "Mouse/Backward");
			}
		}
	}
}
