using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

namespace SoulboundBackend.Client.Input {
	public readonly struct InputToken : IEqualityComparer<InputToken> {
		public readonly Guid guid;
		public readonly string actionName;
		public readonly string mapName;

		public InputToken(InputAction action)
			: this(action.id, action.name, action.actionMap?.name ?? string.Empty) {
		}

		public InputToken(Guid guid, string actionName, string mapName) {
			this.guid = guid;
			this.actionName = actionName;
			this.mapName = mapName;
		}

		public bool Equals(InputToken x, InputToken y) {
			return x.guid.Equals(y.guid);
		}

		public int GetHashCode(InputToken obj) {
			return guid.GetHashCode();
		}

		public override string ToString() {
			return $"{mapName}/{actionName}({guid})";
		}
	}
}
