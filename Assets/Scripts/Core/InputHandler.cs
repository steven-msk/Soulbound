using System.Collections;
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

	private void Awake() {
		inputActions = new PlayerInputActions();
		PlayerActions playerActions = inputActions.Player;

		playerActions.MousePosition.performed += actionContext => MouseScreenPosition = actionContext.ReadValue<Vector2>();

		static IEnumerator OnLeftClick(InputAction.CallbackContext actionContext) {
			yield return null;
			GameManager.GetPlayerInstance().OnLeftClick(actionContext);
		}

		playerActions.LeftClick.performed += actionContext => StartCoroutine(OnLeftClick(actionContext));
		playerActions.LeftClick.performed += actionContext => LeftHold = true;
		playerActions.LeftClick.canceled += actionContext => LeftHold = false;

		playerActions.RightClick.performed += GameManager.GetPlayerInstance().OnRightClick;
		playerActions.RightClick.performed += actionContext => RightHold = true;
		playerActions.RightClick.canceled += actionContext => RightHold = false;

		playerActions.Move.performed += actionContext => {
			Vector2 movement = actionContext.ReadValue<Vector2>();
			HorizontalMovement = movement.x;
		};
		playerActions.Move.canceled += actionContext => HorizontalMovement = 0;

		playerActions.Jump.performed += GameManager.GetPlayerInstance().OnSpacePressed;
		playerActions.Jump.performed += actionContext => PressingSpace = true;
		playerActions.Jump.canceled += actionContext => PressingSpace = false;

		playerActions.ChangeHotbarSlot.performed += actionContext => {
			int keySlot = int.Parse(actionContext.control.name);
			GameManager.GetPlayerInstance().Inventory.Hotbar.SetActiveSlot(keySlot);
		};
		playerActions.ScrollHotbarSlot.performed += actionContext => {
			float scrollDelta = actionContext.ReadValue<float>();
			GameManager.GetPlayerInstance().Inventory.Hotbar.OnHotbarScroll(scrollDelta);
		};

		playerActions.ToggleInventory.performed += GameManager.GetPlayerInstance().Inventory.ToggleInventory;

		playerActions.DropItem.performed += GameManager.GetPlayerInstance().Inventory.DropItemFromInventory;
	}

	private void OnEnable() => inputActions.Enable();

	private void OnDisable() => inputActions.Disable();
}