using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Concurrency {
	public sealed class ActionRequest {
		public Action action { get; }
		public List<Func<bool>> conditions { get; }
		public int priority { get; }

		public ActionRequest(ActionRequestData data) {
			this.action = data.action;
			this.conditions = data.conditions;
			this.priority = data.priority;
		}
	}
}
