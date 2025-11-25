using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Concurrency {
	public class SuppressionBinder : AbstractActionRequestBinder {
		protected SuppressionBinder(ActionRequestData actionRequestData) 
			: base(actionRequestData) {
		}

		public SuppressionBinder Suppress(ActionToken token, Func<bool> unsuppressCondition) {
			actionRequestData.suppressions[token] = unsuppressCondition;
			return this;
		}
	}
}
