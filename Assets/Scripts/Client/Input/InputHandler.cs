using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerInputActions;

public class InputHandler : MonoBehaviour {

	private PlayerInputActions inputActions;

	public Vector2 MouseScreenPosition { get; private set; }
	public Vector2 MouseWorldPosition => Camera.main.ScreenToWorldPoint(MouseScreenPosition);
	public bool LeftHold { get; private set; }
	public bool RightHold { get; private set; }
	public float HorizontalMovement { get; private set; }
	public bool PressingSpace { get; private set; }

	private static List<InputActionRequest> intents = new();
	private static Dictionary<string, Func<bool>> blockedContexts = new();

	private void Awake() {
		inputActions = new PlayerInputActions();
		PlayerActions playerActions = inputActions.Player;
		PlayerController player = GameManager.GetPlayerInstance();

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

		playerActions.Jump.performed += player.OnSpacePressed;
		playerActions.Jump.performed += actionContext => PressingSpace = true;
		playerActions.Jump.canceled += actionContext => PressingSpace = false;

		playerActions.ChangeHotbarSlot.performed += actionContext => {
			int keySlot = int.Parse(actionContext.control.name);
			player.Inventory.Hotbar.SetActiveSlot(keySlot);
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
		intents.Add(intent);
	}

	private void Update() {
		if (LeftHold) {
			GameManager.GetPlayerInstance().OnLeftHold();
		} else if (RightHold) {
			GameManager.GetPlayerInstance().OnRightHold();
		}
	}

	private void LateUpdate() {
		var chosen = intents.OrderByDescending(intent => intent.Priority).FirstOrDefault();
		chosen?.Action.Invoke();
		intents.Clear();
	}

	private void OnEnable() => inputActions.Enable();

	private void OnDisable() => inputActions.Disable();
}