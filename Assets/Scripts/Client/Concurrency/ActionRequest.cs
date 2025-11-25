using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Concurrency {
	public readonly struct ActionRequest {
		public readonly Action action;
		public readonly List<Func<bool>> conditions;
		public readonly int priority;
		public readonly PriorityType priorityType;
		public readonly ActionToken token;
		public readonly Dictionary<ActionToken, Func<bool>> suppressions;

		public ActionRequest(ActionRequestData data) {
			this.action = data.action;
			this.conditions = data.conditions;
			this.priority = data.priority;
			this.priorityType = data.priorityType;
			this.token = data.token;
			this.suppressions = data.suppressions;
		}
	}
}
