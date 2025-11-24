using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Concurrency {
	public sealed class ActionRequestData {
		public Action action;
		public readonly List<Func<bool>> conditions = new();
		public int priority = 0;
		public PriorityType priorityType = PriorityType.Exclusive;
	}
}
