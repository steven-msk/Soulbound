using System;

namespace SoulboundEngine.Client.Input {
	public sealed record InputEventListener(InputToken token, InputEvent.Phase phase, Func<InputEvent, InputHandleResult> callback, int priority = 0) {
		public static InputEventListener Performed(InputToken token, Func<InputEvent, InputHandleResult> callback, int priority = 0) {
			return new InputEventListener(token, InputEvent.Phase.Performed, callback, priority);
		}

		public static InputEventListener Started(InputToken token, Func<InputEvent, InputHandleResult> callback, int priority = 0) {
			return new InputEventListener(token, InputEvent.Phase.Started, callback, priority);
		}

		public static InputEventListener Canceled(InputToken token, Func<InputEvent, InputHandleResult> callback, int priority = 0) {
			return new InputEventListener(token, InputEvent.Phase.Canceled, callback, priority);
		}

		public static InputEventListener ConsumePerformed(InputToken token, Action<InputEvent> callback, int priority = 0) {
			return new InputEventListener(token, InputEvent.Phase.Performed, inputEvent => {
				callback(inputEvent);
				return InputHandleResult.Consume;
			}, priority);
		}

		public static InputEventListener ConsumeStarted(InputToken token, Action<InputEvent> callback, int priority = 0) {
			return new InputEventListener(token, InputEvent.Phase.Started, inputEvent => {
				callback(inputEvent);
				return InputHandleResult.Consume;
			}, priority);
		}

		public static InputEventListener ConsumeCanceled(InputToken token, Action<InputEvent> callback, int priority = 0) {
			return new InputEventListener(token, InputEvent.Phase.Canceled, inputEvent => {
				callback(inputEvent);
				return InputHandleResult.Consume;
			}, priority);
		}

		public static InputEventListener ObserveAny(InputToken token, Action<InputEvent> callback, int priority = 0) {
			return new InputEventListener(token, InputEvent.Phase.Any, inputEvent => {
				callback(inputEvent);
				return InputHandleResult.ObserveOnly;
			}, priority);
		}

		public static InputEventListener ConsumeAny(InputToken token, Action<InputEvent> callback, int priority = 0) {
			return new InputEventListener(token, InputEvent.Phase.Any, inputEvent => {
				callback(inputEvent);
				return InputHandleResult.Consume;
			}, priority);
		}
	}
}
