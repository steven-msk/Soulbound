using SoulboundEngine.World.Player;
using SoulboundEngine.Client.Settings;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Keyboard = SoulboundEngine.Client.Input.Keyboard;

namespace SoulboundEngine.Client {
	public class ClientPlayerInputHandler {
		private readonly SoulboundClient client;
		private static readonly Dictionary<Key, int> HOTBAR_KEYS = new() {
			[Key.Digit1] = 0, [Key.Digit2] = 1, [Key.Digit3] = 2, [Key.Digit4] = 3, [Key.Digit5] = 4,
			[Key.Digit6] = 6, [Key.Digit7] = 6, [Key.Digit8] = 7, [Key.Digit9] = 8
		};
		private bool isHoldingLeft;
		private bool isHoldingRight;
		private bool startedHoldingLeftThisTick;
		private bool startedHoldingRightThisTick;

		public ClientPlayerInputHandler(SoulboundClient client) {
			this.client = client;
		}

		public void Handle(PlayerEntity player, bool shouldBlockKeyboardActions, bool shouldBlockMouse) {
			this.UpdateScreenPointerPos(player);

			if (!shouldBlockMouse) {
				this.HandleMouseClicks(player);
				this.DoHotbarScroll(player);
			}

			if (!shouldBlockKeyboardActions) {
				this.HandleJump(player);
				this.HandleMovement(player);
				this.CheckHotbarKeyPressed(player);
				this.HandleStackThrow(player);
				this.HandleInventoryToggle(player);
			}
		}

		private void CheckHotbarKeyPressed(PlayerEntity player) {
			foreach (var (key, index) in HOTBAR_KEYS) {
				if (this.client.InputManager.keyboard.IsPressed(Keyboard.GetControl(key))) {
					player.SetMainSlot(index);
					return;
				}
			}
		}

		private void DoHotbarScroll(PlayerEntity player) {
			float scrollDelta = this.client.InputManager.mouse.scrollDelta;
			int nextSlot = player.GetMainSlot() - (int)scrollDelta;
			if (nextSlot == player.GetMainSlot()) return; 

			if (nextSlot < 0) nextSlot += PlayerInventory.HOTBAR_SIZE;
			nextSlot %= PlayerInventory.HOTBAR_SIZE;
			player.SetMainSlot(nextSlot);
		}

		private void UpdateScreenPointerPos(PlayerEntity player) {
			Vector2 pointerPos = this.client.InputManager.mouse.mousePos;
			player.SetScreenPointerPos(pointerPos);
		}

		private void HandleMouseClicks(PlayerEntity player) {
			bool previouslyHoldingLeft = this.isHoldingLeft;
			this.isHoldingLeft = this.client.InputManager.mouse.isLeftPressed;
			this.startedHoldingLeftThisTick = !previouslyHoldingLeft && this.isHoldingLeft;
			player.SetHoldingLeft(!this.startedHoldingLeftThisTick && this.isHoldingLeft);
			if (this.startedHoldingLeftThisTick) {
				player.OnLeftClick();
			}

			bool preivouslyHoldingRight = this.isHoldingRight;
			this.isHoldingRight = this.client.InputManager.mouse.isRightPressed;
			this.startedHoldingRightThisTick = !preivouslyHoldingRight && this.isHoldingRight;
			player.SetHoldingRight(!this.startedHoldingRightThisTick && this.isHoldingRight);
			if (this.startedHoldingRightThisTick) {
				player.OnRightClick();
			}
		}

		private void HandleMovement(PlayerEntity player) {
			float movementX = 0f;
			if (GameSettings.keybinds.moveLeft.IsPressed()) movementX -= 1f;
			if (GameSettings.keybinds.moveRight.IsPressed()) movementX += 1f;
			player.SetNormalVelocityX(movementX);
		}

		private void HandleJump(PlayerEntity player) {
			player.SetJumping(GameSettings.keybinds.jump.IsPressed());
		}

		private void HandleStackThrow(PlayerEntity player) {
			// TODO: handle continuous item throw
			while (GameSettings.keybinds.throwItem.WasPressed()) {
				bool ctrl = this.client.InputManager.keyboard.IsPressed(Keyboard.GetControl(Key.LeftCtrl));
				player.ThrowFromMainHand(ctrl);
			}
		}

		private void HandleInventoryToggle(PlayerEntity player) {
			if (GameSettings.keybinds.toggleInventory.WasPressed()) {
				player.ToggleInventory();
			}
		}
	}
}
