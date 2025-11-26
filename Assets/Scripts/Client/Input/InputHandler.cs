using SoulboundBackend.Common;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Bootstrap;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace SoulboundBackend.Client.Input {
	public class InputHandler {
		private readonly List<InputActionRequest> requests = new();
		private readonly Dictionary<string, Func<bool>> blockedContexts = new();
		private readonly Dictionary<InputAction, List<Action<InputAction.CallbackContext>>> registeredCallbacks = new();
		private readonly List<InputAction> pausableInputs = new();
		private readonly InputActionMap actionMap;
		private InputAction currentlyRegistering;

		public InputHandler(InputActionAsset asset, string actionMapId) {
			this.actionMap = asset.FindActionMap(actionMapId);
		}

		public InputHandler(InputActionMap actionMap) {
			this.actionMap = actionMap;
		}

		public InputAction GetAction(string actionId) {
			return actionMap.FindAction(actionId, true);
		}

		public void RegisterInputEvent(InputAction inputAction, bool pausable, Action<InputCallbackBuilder> callbackBinder) {
			if (pausable) {
				pausableInputs.Add(inputAction);
			}
			InputCallbackBuilder callbackBuilder = new(inputAction);
			callbackBinder.Invoke(callbackBuilder);

			var mapping = callbackBuilder.GetCallbacks();
			if (!registeredCallbacks.TryAdd(mapping.Key, mapping.Value)) {
				registeredCallbacks[mapping.Key].AddRange(mapping.Value);
			}
		}

		public void PauseInputs(bool pause) {
			Action<InputAction> action = pause
				? inputAction => inputAction.Disable()
				: inputAction => inputAction.Enable();
			pausableInputs.ForEach(action);
		}

		public void FlushCallbacks() {
			foreach (var kvp in registeredCallbacks) {
				var inputAction = kvp.Key;
				var callbacks = kvp.Value;

				foreach (var callback in callbacks) {
					inputAction.performed -= callback;
					inputAction.canceled -= callback;
					inputAction.started -= callback;
				}
			}
		}

	}
}