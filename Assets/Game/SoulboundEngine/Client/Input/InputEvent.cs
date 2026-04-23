using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

namespace SoulboundEngine.Client.Input {
	public readonly struct InputEvent {
		public readonly InputToken token;
		public readonly InputAction.CallbackContext context;
		public readonly InputActionPhase phase;

		public InputEvent(InputToken token, InputActionPhase phase, InputAction.CallbackContext context) {
			this.token = token;
			this.phase = phase;
			this.context = context;
		}

		public bool Performed(InputToken token) {
			return this.token.Equals(token) && phase == InputActionPhase.Performed;
		}

		public bool Started(InputToken token) {
			return this.token.Equals(token) && phase == InputActionPhase.Started;
		}

		public bool Canceled(InputToken token) {
			return this.token.Equals(token) && phase == InputActionPhase.Canceled;
		}

		public bool Waiting(InputToken token) {
			return this.token.Equals(token) && phase == InputActionPhase.Waiting;
		}

		public bool Disabled(InputToken token) {
			return this.token.Equals(token) && phase == InputActionPhase.Disabled;
		}
	}
}
