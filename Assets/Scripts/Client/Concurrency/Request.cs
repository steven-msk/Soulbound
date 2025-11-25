using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Concurrency {
	public static class Request {
		public static ActionBinder New() => new ActionBinder();
		public static ConditionBinder New(Action action) => new ActionBinder().Execute(action);
	}
}
