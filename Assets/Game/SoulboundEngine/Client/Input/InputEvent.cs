using System;
using UnityEngine.InputSystem;

namespace SoulboundEngine.Client.Input {
	public readonly struct InputEvent {
		public readonly InputToken token;
		public readonly InputAction.CallbackContext context;
		public readonly Phase phase;

		public InputEvent(InputToken token, Phase phase, InputAction.CallbackContext context) {
			this.token = token;
			this.phase = phase;
			this.context = context;
		}

		public bool Performed(InputToken token) {
			return this.token.Equals(token) && this.phase == Phase.Performed;
		}

		public bool Started(InputToken token) {
			return this.token.Equals(token) && this.phase == Phase.Started;
		}

		public bool Canceled(InputToken token) {
			return this.token.Equals(token) && this.phase == Phase.Canceled;
		}

		public override string ToString() {
			return $"input_event[token={this.token}, phase={this.phase}]";
		}

		[Flags]
		public enum Phase {
			Performed	= 1 << 0,
			Started		= 1 << 1,
			Canceled	= 1 << 2,
			Any = Performed | Started | Canceled
		}
	}
}
