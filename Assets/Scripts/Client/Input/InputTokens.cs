using SoulboundBackend.Core;
using System;
using UnityEngine.InputSystem;

namespace SoulboundBackend.Client.Input {
	public static class InputTokens {
		public static readonly InputToken toggleDebugConsole;
		public static readonly InputToken enterCommand;

		[Obsolete] public static readonly InputToken p_jump;
		[Obsolete] public static readonly InputToken p_playerEsc;
		[Obsolete] public static readonly InputToken p_toggleInventory;
		[Obsolete] public static readonly InputToken p_scrollHotbarSlot;
		[Obsolete] public static readonly InputToken p_changeHotbarSlot;
		[Obsolete] public static readonly InputToken p_mousePosition;
		[Obsolete] public static readonly InputToken p_leftClick;
		[Obsolete] public static readonly InputToken p_rightClick;
		[Obsolete] public static readonly InputToken p_move;

		static InputTokens() {
			InputActionAsset asset = Soulbound.instance.GetInputActionAsset();

			toggleDebugConsole = new InputToken(asset.FindAction("Debug/ToggleConsole"));
			enterCommand = new InputToken(asset.FindAction("Debug/EnterCommand"));

			p_jump = new InputToken(asset.FindAction("Player/Jump"));
			p_playerEsc = new InputToken(asset.FindAction("Player/Esc"));
			p_toggleInventory = new InputToken(asset.FindAction("Player/Toggle Inventory"));
			p_scrollHotbarSlot = new InputToken(asset.FindAction("Player/Scroll Hotbar Slot"));
			p_changeHotbarSlot = new InputToken(asset.FindAction("Player/Change Hotbar Slot"));
			p_mousePosition = new InputToken(asset.FindAction("Player/MousePosition"));
			p_leftClick = new InputToken(asset.FindAction("Player/LeftClick"));
			p_rightClick = new InputToken(asset.FindAction("Player/RightClick"));
			p_move = new InputToken(asset.FindAction("Player/Move"));
		}
	}
}
