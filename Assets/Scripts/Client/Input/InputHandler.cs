using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using NUnit.Compatibility;
using NUnit.Framework;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerInputActions;

public class InputHandler : MonoBehaviour {
	public PlayerInputActions inputActions { get; private set; }

	public Vector2 MouseScreenPosition { get; private set; }
	public Vector2 MouseWorldPosition {
		get {
			Vector3 screenPos = MouseScreenPosition;
			RectTransform rootTransform = GameManager.instance.UI.GetRootTransform();
			if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rootTransform, MouseScreenPosition, Camera.main, out var worldPoint)) {
				return worldPoint;
			}
			screenPos.z = -Camera.main.transform.position.z; 
			return Camera.main.ScreenToWorldPoint(screenPos);
		}
	}

	public Vector2 MouseLocalPosition => this.ScreenPosToLocalPos(Input.mousePosition);

	public bool LeftHold { get; private set; }
	public bool RightHold { get; private set; }
	public float HorizontalMovement { get; private set; }
	public bool PressingSpace { get; private set; }

	private static List<InputActionRequest> requests = new();
	private static Dictionary<string, Func<bool>> blockedContexts = new();

	private void Awake() {
		requests.Clear();
		blockedContexts.Clear();

		inputActions = new PlayerInputActions();
		PlayerActions playerActions = inputActions.Player;
		PlayerController player = GameManager.instance.Player;

		playerActions.MousePosition.performed += actionContext => MouseScreenPosition = actionContext.ReadValue<Vector2>();

		playerActions.LeftClick.performed += actionContext => player.OnLeftClick();
		playerActions.LeftClick.performed += actionContext => LeftHold = true;
		playerActions.LeftClick.canceled += actionContext => LeftHold = false;

		playerActions.RightClick.performed += actionContext => player.OnRightClick();
		playerActions.RightClick.performed += actionContext => RightHold = true;
		playerActions.RightClick.canceled += actionContext => RightHold = false;

		playerActions.Move.performed += actionContext => {
			Vector2 movement = actionContext.ReadValue<Vector2>();
			HorizontalMovement = movement.x;
		};
		playerActions.Move.canceled += actionContext => HorizontalMovement = 0;

		playerActions.Jump.performed += actionContext => player.Physics.OnSpacePressed();
		playerActions.Jump.performed += actionContext => PressingSpace = true;
		playerActions.Jump.canceled += actionContext => PressingSpace = false;

		playerActions.ChangeHotbarSlot.performed += actionContext => {
			int keySlot = int.Parse(actionContext.control.name);
			player.Inventory.Hotbar.SetActiveSlot(keySlot - 1);
		};
		playerActions.ScrollHotbarSlot.performed += actionContext => {
			float scrollDelta = actionContext.ReadValue<float>();
			player.Inventory.Hotbar.OnHotbarScroll(scrollDelta);
		};

		playerActions.ToggleInventory.performed += player.Inventory.ToggleInventory;

		playerActions.DropItem.performed += player.Inventory.DropItemFromInventory;
	}

	public static void BlockContextUntil(string context, Func<bool> unblockPredicate) => blockedContexts[context] = unblockPredicate;

	public static void RequestAction(InputActionRequest intent) {
		if (blockedContexts.TryGetValue(intent.Context, out var predicate)) {
			if (predicate.Invoke()) {
				return;
			}
			blockedContexts.Remove(intent.Context);
		}
		requests.Add(intent);
	}

	private void Update() {
		if (LeftHold) {
			GameManager.instance.Player.OnLeftHold();
		} else if (RightHold) {
			GameManager.instance.Player.OnRightHold();
		}
	}

	private void LateUpdate() {
		var chosen = requests.OrderByDescending(intent => intent.Priority).FirstOrDefault();
		chosen?.Action.Invoke();
		requests.Clear();
	}

	public Vector2 ScreenPosToLocalPos(Vector2 screenPos) {
		RectTransform rootTransfom = GameManager.instance.UI.GetRootTransform();
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rootTransfom, screenPos, GameManager.instance.UI.Canvas.worldCamera, out var localPos)) {
			return localPos;
		}
		Debug.LogError($"Could not retrieve local point from screen point: ({screenPos.x}, {screenPos.y})");
		return new(-1f, -1f);
	}

	private void OnEnable() => inputActions.Enable();

	private void OnDisable() => inputActions.Disable();
}