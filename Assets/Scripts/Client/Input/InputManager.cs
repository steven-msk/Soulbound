using SoulboundBackend.Common;
using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Logger = SoulboundBackend.Core.Debug.Logging.Logger;

namespace SoulboundBackend.Client.Input {
	public sealed class InputManager {
		private readonly InputActionAsset inputAsset;
		private readonly List<IInputContext> contextStack = new();
		private readonly Dictionary<Guid, InputToken> tokenRegistry = new();
		private readonly BufferedQueue<InputEvent> eventQueue = new(128);

		public InputManager(InputActionAsset inputAsset) {
			this.inputAsset = inputAsset;
			BindAllActions();
			inputAsset.Enable();
		}

		private void BindAllActions() {
			foreach (var actionMap in inputAsset.actionMaps) {
				foreach (var inputAction in actionMap.actions) {
					inputAction.performed += ctx => PendEvent(inputAction, ctx);
					inputAction.started += ctx => PendEvent(inputAction, ctx);
					inputAction.canceled += ctx => PendEvent(inputAction, ctx);
					tokenRegistry[inputAction.id] = new InputToken(inputAction);
				}
			}
		}

		private void PendEvent(InputAction action, InputAction.CallbackContext context) {
			InputEvent inputEvent = new(tokenRegistry[action.id], context.phase, context);
			if (!eventQueue.TryEnqueue(in inputEvent)) {
				Logger.LogWarning("Input queue capacity exceeded! Failed to enqueue action '{}' phase '{}'", action.name, context.phase);
			}
		}

		public void DispatchInputs() {
			while (eventQueue.TryDequeue(out InputEvent inputEvent)) {
				Dispatch(in inputEvent);
			}
		}

		private void Dispatch(in InputEvent inputEvent) {
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
