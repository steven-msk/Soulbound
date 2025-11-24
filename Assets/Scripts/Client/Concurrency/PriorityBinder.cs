using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Concurrency {
	public class PriorityBinder : PriorityTypeBinder {
		protected PriorityBinder(ActionRequestData actionRequestData) 
			: base(actionRequestData) {
		}

		public PriorityTypeBinder WithPriority(int priority) {
			actionRequestData.priority = priority;
			return this;
		}
	}
}
