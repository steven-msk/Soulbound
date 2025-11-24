using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Concurrency {
	public class ConditionChainBinder : PriorityBinder {
		public ConditionChainBinder(ActionRequestData actionRequestData) : base(actionRequestData) {
		}

		public ConditionChainBinder And(Func<bool> condition) {
			if (condition == null) {
				throw new ArgumentNullException("Condition cannot be null");
			}
			actionRequestData.conditions.Add(condition);
			return this;
		}
	}
}
