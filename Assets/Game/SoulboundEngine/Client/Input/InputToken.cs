using System;
using UnityEngine.InputSystem;

namespace SoulboundEngine.Client.Input {
	public readonly struct InputToken {
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

		public override bool Equals(object obj) {
			return obj is InputToken token && this.guid.Equals(token.guid);
		}

		public override int GetHashCode() => this.guid.GetHashCode();

		public override string ToString() {
			return $"{this.mapName}/{this.actionName}";
		}
	}
}
