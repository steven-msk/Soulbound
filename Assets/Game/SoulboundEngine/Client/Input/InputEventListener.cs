using System;

namespace SoulboundEngine.Client.Input {
	public sealed record InputEventListener(InputToken token, InputEvent.Phase phase, Predicate<InputEvent> callback, int priority = 0) {
		public static InputEventListener Performed(InputToken token, Predicate<InputEvent> callback, int priority = 0) {
			return new InputEventListener(token, InputEvent.Phase.Performed, callback, priority);
		}

		public static InputEventListener Started(InputToken token, Predicate<InputEvent> callback, int priority = 0) {
			return new InputEventListener(token, InputEvent.Phase.Started, callback, priority);
		}

		public static InputEventListener Canceled(InputToken token, Predicate<InputEvent> callback, int priority = 0) {
			return new InputEventListener(token, InputEvent.Phase.Canceled, callback, priority);
		}

		public static InputEventListener ConsumePerformed(InputToken token, Action<InputEvent> callback, int priority = 0) {
			return new InputEventListener(token, InputEvent.Phase.Performed, inputEvent => {
				callback(inputEvent);
				return true;
			}, priority);
		}

		public static InputEventListener ConsumeStarted(InputToken token, Action<InputEvent> callback, int priority = 0) {
			return new InputEventListener(token, InputEvent.Phase.Started, inputEvent => {
				callback(inputEvent);
				return true;
			}, priority);
		}

		public static InputEventListener ConsumeCanceled(InputToken token, Action<InputEvent> callback, int priority = 0) {
			return new InputEventListener(token, InputEvent.Phase.Canceled, inputEvent => {
				callback(inputEvent);
				return true;
			}, priority);
		}
	}
}
