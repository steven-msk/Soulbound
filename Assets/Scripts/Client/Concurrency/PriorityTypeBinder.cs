using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Concurrency {
	public class PriorityTypeBinder : AbstractActionRequestBinder {
		protected PriorityTypeBinder(ActionRequestData actionRequestData) 
			: base(actionRequestData) {
		}

		public AbstractActionRequestBinder Exclusive() {
			actionRequestData.priorityType = PriorityType.Exclusive;
			return this;
		}

		public AbstractActionRequestBinder NonExclusive() {
			actionRequestData.priorityType = PriorityType.NonExclusive;
			return this;
		}
	}
}
