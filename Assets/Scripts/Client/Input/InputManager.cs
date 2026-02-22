using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SoulboundBackend.Client.Input {
	public sealed class InputManager {
		private readonly InputActionAsset inputAsset;
		private readonly List<IInputContext> contextStack = new();
		private readonly Dictionary<Guid, InputToken> tokenRegistry = new();

		public InputManager(InputActionAsset inputAsset) {
			this.inputAsset = inputAsset;
			BindAllActions();
			inputAsset.Enable();
		}

		private void BindAllActions() {
			foreach (var actionMap in inputAsset.actionMaps) {
				foreach (var inputAction in actionMap.actions) {
					inputAction.performed += ctx => Dispatch(inputAction, ctx);
					inputAction.started += ctx => Dispatch(inputAction, ctx);
					inputAction.canceled += ctx => Dispatch(inputAction, ctx);
					tokenRegistry[inputAction.id] = new InputToken(inputAction);
				}
			}
		}

		private void Dispatch(InputAction action, InputAction.CallbackContext context) {
			InputEvent inputEvent = new(tokenRegistry[action.id], context.phase, context);

			for (int i = contextStack.Count - 1; i >= 0; i--) {
				if (contextStack[i].HandleInput(in inputEvent)) return;
			}
		}

		public void PushContext(IInputContext inputContext) {
			contextStack.Add(inputContext);
			contextStack.Sort((a, b) => a.priority.CompareTo(b.priority));
		}

		public void RemoveContext(IInputContext inputContext) {
			contextStack.Remove(inputContext);
		}
	}
}
