namespace SoulboundEngine.UnityClient.Input {
	using System;
	using UnityEngine;
	using UnityEngine.InputSystem;

	public sealed class Mouse : InputManager.MappedInputActions {
		public const string ACTION_MAP = "Mouse";
		public const string LEFT_PATH = "LeftClick";
		public const string RIGHT_PATH = "RightClick";
		public const string POSITION_PATH = "Position";
		public const string FORWARD_PATH = "Forward";
		public const string BACK_PATH = "Backward";
		public const string SCROLL_PATH = "Scroll";
		public event Action<Vector2> mouseMoved;
		private int leftClicks;
		private int rightClicks;
		private int forwardClicks;
		private int backClicks;

		public Mouse(InputActionAsset asset)
			: base(asset) {
			InputAction leftClick = this.map.FindAction(LEFT_PATH, true);
			leftClick.started += this.OnLeftPressed;
			leftClick.canceled += this.OnLeftReleased;

			InputAction rightClick = this.map.FindAction(RIGHT_PATH, true);
			rightClick.started += this.OnRightPressed;
			rightClick.canceled += this.OnRightReleased;

			InputAction forward = this.map.FindAction(FORWARD_PATH, true);
			forward.started += this.OnForwardPressed;
			forward.canceled += this.OnForwardReleased;

			InputAction back = this.map.FindAction(BACK_PATH, true);
			back.started += this.OnBackPressed;
			back.canceled += this.OnBackReleased;

			InputAction position = this.map.FindAction(POSITION_PATH, true);
			position.performed += this.MouseMoved;

			InputAction scroll = this.map.FindAction(SCROLL_PATH, true);
			scroll.performed += this.Scrolled;
		}

		protected override InputActionMap GetMap(InputActionAsset asset) {
			return asset.FindActionMap(ACTION_MAP, throwIfNotFound: true);
		}

		internal protected override void Tick() {
			this.leftClicks = 0;
			this.rightClicks = 0;
			this.forwardClicks = 0;
			this.backClicks = 0;
			this.scrollDelta = 0;
		}

		public bool isLeftPressed { get; private set; }
		public bool isRightPressed { get; private set; }
		public bool isForwardPressed { get; private set; }
		public bool isBackPressed { get; private set; }
		public Vector2 position { get; private set; }
		public float scrollDelta { get; private set; }

		public bool WasLeftPressed() {
			if (this.leftClicks <= 0) return false;
			this.leftClicks--;
			return true;
		}

		public bool WasRightPressed() {
			if (this.rightClicks <= 0) return false;
			this.rightClicks--;
			return true;
		}

		public bool WasForwardPressed() {
			if (this.forwardClicks <= 0) return false;
			this.forwardClicks--;
			return true;
		}

		public bool WasBackPressed() {
			if (this.backClicks <= 0) return false;
			this.backClicks--;
			return true;
		}

		private void OnLeftPressed(InputAction.CallbackContext ctx) {
			this.isLeftPressed = true;
			this.leftClicks++;
		}

		private void OnLeftReleased(InputAction.CallbackContext ctx) {
			this.isLeftPressed = false;
		}

		private void OnRightPressed(InputAction.CallbackContext ctx) {
			this.isRightPressed = true;
			this.rightClicks++;
		}

		private void OnRightReleased(InputAction.CallbackContext ctx) {
			this.isRightPressed = false;
		}

		private void OnForwardPressed(InputAction.CallbackContext ctx) {
			this.isForwardPressed = true;
			this.forwardClicks++;
		}

		private void OnForwardReleased(InputAction.CallbackContext ctx) {
			this.isForwardPressed = false;
		}

		private void OnBackPressed(InputAction.CallbackContext ctx) {
			this.isBackPressed = true;
			this.backClicks++;
		}

		private void OnBackReleased(InputAction.CallbackContext ctx) {
			this.isBackPressed = false;
		}

		private void MouseMoved(InputAction.CallbackContext ctx) {
			this.position = ctx.ReadValue<Vector2>();
			mouseMoved?.Invoke(this.position);
		}

		private void Scrolled(InputAction.CallbackContext ctx) {
			this.scrollDelta = ctx.ReadValue<float>();
		}
	}
}
