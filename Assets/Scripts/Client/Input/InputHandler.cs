using SoulboundBackend.Common;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Bootstrap;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using static PlayerInputActions;

namespace SoulboundBackend.Client.Input {
	public class InputHandler : ITickable, ILateTickable {
		private List<InputActionRequest> requests = new();
		private Dictionary<string, Func<bool>> blockedContexts = new();
		private List<InputAction> pausableInputs = new();

		public void RegisterInputEvent(InputAction inputAction, bool pausable, Action<InputAction> callbackBinding) {
			if (pausable) {
				pausableInputs.Add(inputAction);
			}
			callbackBinding.Invoke(inputAction);
		}

		public void BlockContext(string context, Func<bool> unblockPredicate) => blockedContexts[context] = unblockPredicate;

		public void RequestAction(InputActionRequest action) => requests.Add(action);

		void ITickable.Tick() {
			List<string> unblockedPersistent = new();
			foreach (var kvp in blockedContexts) {
				if (kvp.Value.Invoke()) {
					unblockedPersistent.Add(kvp.Key);
				}
			}
			unblockedPersistent.ForEach(context => blockedContexts.Remove(context));
		}

		void ILateTickable.LateTick() {
			var availableRequests = requests.Where(action => !blockedContexts.ContainsKey(action.Context));
			InputActionRequest highestPriorityRequest = availableRequests.OrderByDescending(intent => intent.Priority).FirstOrDefault();
			highestPriorityRequest?.Callback.Invoke();
			highestPriorityRequest?.Action.Invoke();
			requests.Clear();
		}

		public Vector2 ScreenPosToLocalPos(Vector2 screenPos) {
			RectTransform rootTransfom = Soulbound.instance.GetActiveLevelManager().UIManager.GetRootTransform();
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rootTransfom, screenPos, Soulbound.instance.GetActiveLevelManager().UIManager.rootCanvas.worldCamera, out var localPos)) {
				return localPos;
			}
			UnityEngine.Debug.LogError($"Could not retrieve local point from screen point: ({screenPos.x}, {screenPos.y})");
			return new(-1f, -1f);
		}

		public void PauseInputs(bool pause) {
			Action<InputAction> action = pause
				? inputAction => inputAction.Disable()
				: inputAction => inputAction.Enable();
			pausableInputs.ForEach(action);
		}
	}
}