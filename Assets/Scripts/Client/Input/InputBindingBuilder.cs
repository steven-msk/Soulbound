using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace SoulboundBackend.Client.Input {
	public class InputBindingBuilder {
		private readonly List<Action<InputAction.CallbackContext>> subscribedDelegates = new();
		private readonly InputAction action;

		public InputBindingBuilder(InputAction action) {
			this.action = action;
		}

		public void Performed(Action<InputAction.CallbackContext> callback) {
			action.performed += callback;
			subscribedDelegates.Add(callback);
		}

		public void Canceled(Action<InputAction.CallbackContext> callback) {
			action.canceled += callback;
			subscribedDelegates.Add(callback);
		}

		public void Started(Action<InputAction.CallbackContext> callback) {
			action.started += callback;
			subscribedDelegates.Add(callback);
		}

		public KeyValuePair<InputAction, List<Action<InputAction.CallbackContext>>> GetMapping() {
			return new(action, subscribedDelegates);
		}
	}
}