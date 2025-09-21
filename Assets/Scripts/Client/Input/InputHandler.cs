using SoulboundBackend.Common;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Bootstrap;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerInputActions;

namespace SoulboundBackend.Client.Input {
	[BootstrappableChildOf(typeof(PlayerController))]
	public class InputHandler : MonoBehaviour, IBootstrappable {
		public PlayerInputActions inputActions { get; private set; }

		public Vector2 MouseScreenPosition { get; private set; }
		public Vector2 MouseWorldPosition {
			get {
				Vector3 screenPos = MouseScreenPosition;
				RectTransform rootTransform = LevelManager.instance.UIManager.GetRootTransform();
				if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rootTransform, MouseScreenPosition, Camera.main, out var worldPoint)) {
					return worldPoint;
				}
				screenPos.z = -Camera.main.transform.position.z;
				return Camera.main.ScreenToWorldPoint(screenPos);
			}
		}

		public Vector2 MouseLocalPosition => this.ScreenPosToLocalPos(UnityEngine.Input.mousePosition);

		public bool LeftHold { get; private set; }
		public bool RightHold { get; private set; }
		public float HorizontalMovement { get; private set; }
		public bool PressingSpace { get; private set; }

		private static List<InputActionRequest> requests = new();
		private static Dictionary<string, Func<bool>> blockedContexts = new();
		private static List<InputAction> pausableInputs = new();

		public void OnBootstrap(DependencyContainer dependencyContainer) {
			requests.Clear();
			blockedContexts.Clear();
			pausableInputs.Clear();

			inputActions = new PlayerInputActions();
			PlayerActions playerActions = inputActions.Player;
			PlayerController player = dependencyContainer.Resolve<PlayerController>();

			RegisterInputEvent(playerActions.MousePosition, pausable: false, (action) => {
				action.performed += actionContext => MouseScreenPosition = actionContext.ReadValue<Vector2>();
			});

			RegisterInputEvent(playerActions.LeftClick, pausable: false, (action) => {
				action.performed += actionContext => player.OnLeftClick();
				action.performed += actionContext => LeftHold = true;
				action.canceled += actionContext => LeftHold = false;
			});
			RegisterInputEvent(playerActions.RightClick, pausable: false, (action) => {
				action.performed += actionContext => player.OnRightClick();
				action.performed += actionContext => RightHold = true;
				action.canceled += actionContext => RightHold = false;
			});

			RegisterInputEvent(playerActions.Move, pausable: true, (action) => {
				action.performed += actionContext => HorizontalMovement = actionContext.ReadValue<Vector2>().x;
				action.canceled += actionContext => HorizontalMovement = 0;
			});

			RegisterInputEvent(playerActions.Jump, pausable: true, (action) => {
				action.performed += actionContext => player.Physics.OnSpacePressed();
				action.performed += actionContext => PressingSpace = true;
				action.canceled += actionContext => PressingSpace = false;
			});

			RegisterInputEvent(playerActions.ChangeHotbarSlot, pausable: true, (action) => {
				action.performed += actionContext => {
					int keySlot = int.Parse(actionContext.control.name);
					player.Inventory.Hotbar.SetActiveSlot(keySlot - 1);
				};
			});
			RegisterInputEvent(playerActions.ScrollHotbarSlot, pausable: true, (action) => {
				action.performed += actionContext => {
					float scrollDelta = actionContext.ReadValue<float>();
					player.Inventory.Hotbar.OnHotbarScroll(scrollDelta);
				};
			});

			RegisterInputEvent(playerActions.ToggleInventory, pausable: true, (action) => {
				action.performed += actionContext => player.Inventory.ToggleInventory();
			});

			RegisterInputEvent(playerActions.DropItem, pausable: true, (action) => {
				action.performed += actionContext => player.Inventory.DropHoveredOrActiveItem();
			});

			RegisterInputEvent(playerActions.PauseGame, pausable: false, (action) => {
				action.performed += actionContext => LevelManager.instance.TogglePauseGame();
			});

			inputActions.Enable();
		}

		public void OnEarlyBootstrap(DependencyContainer dependencyContainer) {
			dependencyContainer.Register<InputHandler>(this);
		}

		private static void RegisterInputEvent(InputAction inputAction, bool pausable, Action<InputAction> callbackBinding) {
			if (pausable) {
				pausableInputs.Add(inputAction);
			}
			callbackBinding.Invoke(inputAction);
		}

		public static void BlockContext(string context, Func<bool> unblockPredicate) => blockedContexts[context] = unblockPredicate;

		public static void RequestAction(InputActionRequest action) => requests.Add(action);

		private void Update() {
			InvocationHelper.If(LeftHold, LevelManager.instance.Player.OnLeftHold);
			InvocationHelper.If(RightHold, LevelManager.instance.Player.OnRightHold);

			List<string> unblockedPersistent = new();
			foreach (var kvp in blockedContexts) {
				if (kvp.Value.Invoke()) {
					unblockedPersistent.Add(kvp.Key);
				}
			}
			unblockedPersistent.ForEach(context => blockedContexts.Remove(context));
		}

		private void LateUpdate() {
			var availableRequests = requests.Where(action => !blockedContexts.ContainsKey(action.Context));
			InputActionRequest highestPriorityRequest = availableRequests.OrderByDescending(intent => intent.Priority).FirstOrDefault();
			highestPriorityRequest?.Callback.Invoke();
			highestPriorityRequest?.Action.Invoke();
			requests.Clear();
		}

		public Vector2 ScreenPosToLocalPos(Vector2 screenPos) {
			RectTransform rootTransfom = LevelManager.instance.UIManager.GetRootTransform();
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rootTransfom, screenPos, LevelManager.instance.UIManager.Canvas.worldCamera, out var localPos)) {
				return localPos;
			}
			UnityEngine.Debug.LogError($"Could not retrieve local point from screen point: ({screenPos.x}, {screenPos.y})");
			return new(-1f, -1f);
		}

		public static void PauseInputs() => pausableInputs.ForEach(inputAction => inputAction.Disable());

		public static void UnpauseInputs() => pausableInputs.ForEach(inputAction => inputAction.Enable());
	}

}