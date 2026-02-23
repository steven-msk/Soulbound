using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

namespace SoulboundBackend.Client.Input {
	public static partial class InputTokens {
		public static class Player {
			public static InputToken jump;
			public static InputToken toggleInventory;
			public static InputToken scrollHotbarSlot;
			public static InputToken changeHotbarSlot;
			public static InputToken rightClick;
			public static InputToken move;

			public static void Register(InputActionAsset asset) {
				jump			 = Create(asset, "Player/Jump");
				toggleInventory  = Create(asset, "Player/Toggle Inventory");
				scrollHotbarSlot = Create(asset, "Player/Scroll Hotbar Slot");
				changeHotbarSlot = Create(asset, "Player/Change Hotbar Slot");
				move			 = Create(asset, "Player/Move");
			}
		}
	}
}
