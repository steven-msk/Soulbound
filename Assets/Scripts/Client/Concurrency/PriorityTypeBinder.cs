using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Concurrency {
	public class PriorityTypeBinder : SuppressionBinder {
		protected PriorityTypeBinder(ActionRequestData actionRequestData) 
			: base(actionRequestData) {
		}

		public SuppressionBinder Exclusive() {
			actionRequestData.priorityType = PriorityType.Exclusive;
			return this;
		}

		public SuppressionBinder NonExclusive() {
			actionRequestData.priorityType = PriorityType.NonExclusive;
			return this;
		}
	}
}
