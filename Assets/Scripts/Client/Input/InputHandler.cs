using SoulboundBackend.Common;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Bootstrap;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using static PlayerInputActions;

namespace SoulboundBackend.Client.Input {
	public class InputHandler : ITickable, ILateTickable {
		public PlayerInputActions inputActions { get; private set; }
		private PlayerController player;
		private PlayerActions playerActions;

		public Vector2 MouseScreenPosition { get; private set; }
		public Vector2 MouseWorldPosition {
			get {
				Vector3 screenPos = MouseScreenPosition;
				RectTransform rootTransform = Soulbound.instance.GetActiveLevelManager().UIManager.GetRootTransform();
				if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rootTransform, MouseScreenPosition, Camera.main, out var worldPoint)) { 
					return worldPoint;
				}
				screenPos.z = -Camera.main.transform.position.z;
				return Camera.main.ScreenToWorldPoint(screenPos);
			}
		}

		public Vector2 MouseLocalPosition => this.ScreenPosToLocalPos(UnityEngine.Input.mousePosition);

		public bool LeftHold { get; private set; } = false;
		public bool RightHold { get; private set; } = false;
		public float HorizontalMovement { get; private set; } = 0.0f;
		public bool PressingSpace { get; private set; } = false;

		private List<InputActionRequest> requests = new();
		private Dictionary<string, Func<bool>> blockedContexts = new();
		private List<InputAction> pausableInputs = new();

		public InputHandler() {
			inputActions = new PlayerInputActions();
			playerActions = inputActions.Player;

			//RegisterInputEvent(playerActions.MousePosition, pausable: false, (action) => {
			//	action.performed += actionContext => MouseScreenPosition = actionContext.ReadValue<Vector2>();
			//});

			//RegisterInputEvent(playerActions.LeftClick, pausable: true, (action) => {
			//	action.performed += actionContext => player.OnLeftClick();
			//	action.performed += actionContext => LeftHold = true;
			//	action.canceled += actionContext => LeftHold = false;
			//});
			//RegisterInputEvent(playerActions.RightClick, pausable: true, (action) => {
			//	action.performed += actionContext => player.OnRightClick();
			//	action.performed += actionContext => RightHold = true;
			//	action.canceled += actionContext => RightHold = false;
			//});

			//RegisterInputEvent(playerActions.Move, pausable: true, (action) => {
			//	action.performed += actionContext => HorizontalMovement = actionContext.ReadValue<Vector2>().x;
			//	action.canceled += actionContext => HorizontalMovement = 0;
			//});

			//RegisterInputEvent(playerActions.Jump, pausable: true, (action) => {
			//	action.performed += actionContext => player.Physics.OnSpacePressed();
			//	action.performed += actionContext => PressingSpace = true;
			//	action.canceled += actionContext => PressingSpace = false;
			//});

			//RegisterInputEvent(playerActions.ChangeHotbarSlot, pausable: true, (action) => {
			//	action.performed += actionContext => {
			//		int keySlot = int.Parse(actionContext.control.name);
			//		player.Inventory.Hotbar.SetActiveSlot(keySlot - 1);
			//	};
			//});
			//RegisterInputEvent(playerActions.ScrollHotbarSlot, pausable: true, (action) => {
			//	action.performed += actionContext => {
			//		float scrollDelta = actionContext.ReadValue<float>();
			//		player.Inventory.Hotbar.OnHotbarScroll(scrollDelta);
			//	};
			//});

			//RegisterInputEvent(playerActions.ToggleInventory, pausable: true, (action) => {
			//	action.performed += actionContext => player.Inventory.ToggleInventory();
			//});

			//RegisterInputEvent(playerActions.DropItem, pausable: true, (action) => {
			//	action.performed += actionContext => player.Inventory.DropHoveredOrActiveItem();
			//});

			//RegisterInputEvent(playerActions.Esc, pausable: false, (action) => {
			//	action.performed += actionContext => Soulbound.instance.GetActiveLevelManager().OnEscPressed();
			//});

			inputActions.Enable();
		}

		//[Inject]
		//public void Construct(PlayerController player) {
		//	this.player = player;
		//}

		private void RegisterInputEvent(InputAction inputAction, bool pausable, Action<InputAction> callbackBinding) {
			if (pausable) {
				pausableInputs.Add(inputAction);
			}
			callbackBinding.Invoke(inputAction);
		}

		public void BlockContext(string context, Func<bool> unblockPredicate) => blockedContexts[context] = unblockPredicate;

		public void RequestAction(InputActionRequest action) => requests.Add(action);

		void ITickable.Tick() {
			if (LeftHold) {
				player.OnLeftHold();
			}
			if (RightHold) {
				player.OnRightHold();
			}

			List<string> unblockedPersistent = new();
			foreach (var kvp in blockedContexts) {
				if (kvp.Value.Invoke()) {
					unblockedPersistent.Add(kvp.Key);
				}
			}
			unblockedPersistent.ForEach(context => blockedContexts.Remove(context));
		}

		void ILateTickable.LateTick() {
			var availableRequests = requests.Where(action => !blockedContexts.ContainsKey(action.Context));
			InputActionRequest highestPriorityRequest = availableRequests.OrderByDescending(intent => intent.Priority).FirstOrDefault();
			highestPriorityRequest?.Callback.Invoke();
			highestPriorityRequest?.Action.Invoke();
			requests.Clear();
		}

		public Vector2 ScreenPosToLocalPos(Vector2 screenPos) {
			RectTransform rootTransfom = Soulbound.instance.GetActiveLevelManager().UIManager.GetRootTransform();
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rootTransfom, screenPos, Soulbound.instance.GetActiveLevelManager().UIManager.rootCanvas.worldCamera, out var localPos)) {
				return localPos;
			}
			UnityEngine.Debug.LogError($"Could not retrieve local point from screen point: ({screenPos.x}, {screenPos.y})");
			return new(-1f, -1f);
		}

		public void PauseInputs(bool pause) {
			Action<InputAction> action = pause
				? inputAction => inputAction.Disable()
				: inputAction => inputAction.Enable();
			pausableInputs.ForEach(action);
		}
	}
}