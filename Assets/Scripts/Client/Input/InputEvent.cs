using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

namespace SoulboundBackend.Client.Input {
	public readonly struct InputEvent {
		public readonly InputToken token;
		public readonly InputAction.CallbackContext context;
		public readonly InputActionPhase phase;

		public InputEvent(InputToken token, InputActionPhase phase, InputAction.CallbackContext context) {
			this.token = token;
			this.phase = phase;
			this.context = context;
		}
	}
}
