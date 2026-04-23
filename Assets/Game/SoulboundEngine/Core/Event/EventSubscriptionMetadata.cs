using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Core.Event {
	public struct EventSubscriptionMetadata {
		public Action<object> add;
		public Action<object> remove;
	}
}
