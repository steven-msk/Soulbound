using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Concurrency {
	public class ConditionBinder : PriorityBinder {
		protected ConditionBinder(ActionRequestData actionRequestData) : base(actionRequestData) {
		}

		public ConditionChainBinder OnCondition(Func<bool> condition) {
			if (condition == null) {
				throw new ArgumentNullException("Condition cannot be null");
			}
			actionRequestData.conditions.Add(condition);
			return new ConditionChainBinder(actionRequestData);
		}
	}
}
