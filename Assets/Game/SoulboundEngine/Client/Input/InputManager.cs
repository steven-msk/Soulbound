using SoulboundEngine.Client.SettingSystem;
using SoulboundEngine.Common.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using Logger = SoulboundEngine.Client.Debug.Logging.Logger;

namespace SoulboundEngine.Client.Input {
	public sealed class InputManager : IInputManager {
		private readonly InputActionAsset inputAsset;
		private readonly BufferedQueue<InputEvent> eventQueue;
		private readonly Dictionary<IInputEventHandler, List<InputEventListener>> listenersByHandler = new();
		private readonly Dictionary<InputToken, List<InputEventListener>> tokenToListeners = new();
		private readonly HashSet<InputToken> dirtyEntries = new();

		public InputManager(int queueBufferSize, InputActionAsset inputAsset) {
			this.inputAsset = inputAsset;
			this.eventQueue = new BufferedQueue<InputEvent>(queueBufferSize);

			this.CreateTokens(inputAsset);
			inputAsset.Enable();
		}

		private void CreateTokens(InputActionAsset asset) {
			foreach (var actionMap in asset.actionMaps) {
				foreach (var inputAction in actionMap.actions) {
					InputToken inputToken = new(inputAction);

					inputAction.performed += ctx => this.QueueEvent(inputToken, ctx);
					inputAction.started += ctx => this.QueueEvent(inputToken, ctx);
					inputAction.canceled += ctx => this.QueueEvent(inputToken, ctx);
				}
			}
		}

		private void QueueEvent(InputToken token, InputAction.CallbackContext context) {
			InputEvent.Phase phase = ConvertPhase(context.phase);
			InputEvent inputEvent = new(token, phase, context);
			
			if (!this.eventQueue.TryEnqueue(in inputEvent)) {
				Logger.LogError("Input queue buffer capacity exceeded!");
			}
		}

		public void DispatchInputs() {
			if (this.dirtyEntries.Any()) {
				foreach (var token in this.dirtyEntries) {
					if (this.tokenToListeners.TryGetValue(token, out List<InputEventListener> listeners)) {
						this.SortListenersByAscendingPriority(listeners);
					}
				}
				this.dirtyEntries.Clear();
			}

			// iterate over a copy of the listener dictionary to avoid instant secondary effects from callbacks
			Dictionary<InputToken, List<InputEventListener>> tokenToListenersCopy = this.tokenToListeners
				.ToDictionary(kvp => kvp.Key, kvp => new List<InputEventListener>(kvp.Value));

			while (this.eventQueue.TryDequeue(out InputEvent inputEvent)) {
				if (tokenToListenersCopy.TryGetValue(inputEvent.token, out List<InputEventListener> listeners)) {
					Queue<Func<InputEvent, InputHandleResult>> dispatchQueue = this.GetDispatchQueue(in inputEvent, listeners);

					while (dispatchQueue.TryDequeue(out Func<InputEvent, InputHandleResult> callback)) {
						InputHandleResult result = callback(inputEvent);

						if (result == InputHandleResult.Consume) break;
					}
				}
			}

		}

		private Queue<Func<InputEvent, InputHandleResult>> GetDispatchQueue(in InputEvent inputEvent, List<InputEventListener> listeners) {
			Queue<Func<InputEvent, InputHandleResult>> queue = new();

			// assumes listeners are sorted by ascending priority
			for (int i = listeners.Count - 1; i >= 0; i--) {
				InputEventListener listener = listeners[i];

				if (listener.phase.HasFlag(inputEvent.phase)) {
					queue.Enqueue(listener.callback);
				}
			}

			return queue;
		}

		private void SortListenersByAscendingPriority(List<InputEventListener> listeners) {
			listeners.Sort(Comparer<InputEventListener>.Create((a, b) => a.priority.CompareTo(b.priority)));
		}

		public void AddListener(InputEventListener listener) {
			if (!this.tokenToListeners.ContainsKey(listener.token)) {
				this.tokenToListeners[listener.token] = new List<InputEventListener>();
			}
			this.tokenToListeners[listener.token].Add(listener);
			this.dirtyEntries.Add(listener.token);
		}

		public void RemoveListener(InputEventListener listener) {
			if (this.tokenToListeners.TryGetValue(listener.token, out List<InputEventListener> listeners)) {
				listeners.Remove(listener);
			}
			this.dirtyEntries.Add(listener.token);
		}

		public void AddHandler(IInputEventHandler handler) {
			List<InputEventListener> listeners = handler.GetListeners().ToList();

			if (this.listenersByHandler.TryAdd(handler, listeners)) {
				foreach (var listener in listeners) {
					this.AddListener(listener);
				}
			}
		}

		public void RemoveHandler(IInputEventHandler handler) {
			if (this.listenersByHandler.Remove(handler, out List<InputEventListener> listeners)) {
				foreach (var listener in listeners) {
					this.RemoveListener(listener);
				}
			}
		}

		public void Rebind(InputToken token, KeybindEntry keybind) {
			string id = token.guid.ToString();
			InputAction action = this.inputAsset.FindAction(id, throwIfNotFound: true);

			action.ApplyBindingOverride(0, keybind.value?.path ?? "");
		}

		private static InputEvent.Phase ConvertPhase(InputActionPhase phase) {
			return phase switch {
				InputActionPhase.Performed => InputEvent.Phase.Performed,
				InputActionPhase.Started => InputEvent.Phase.Started,
				InputActionPhase.Canceled => InputEvent.Phase.Canceled,
				_ => throw new ArgumentException($"Input phase not supported: {phase}")
			};
		}
	}
}
