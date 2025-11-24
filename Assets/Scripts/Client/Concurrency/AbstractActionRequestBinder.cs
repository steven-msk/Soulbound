using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Concurrency {
	public abstract class AbstractActionRequestBinder {
		protected readonly ActionRequestData actionRequestData;

		protected AbstractActionRequestBinder(ActionRequestData actionRequestData) {
			this.actionRequestData = actionRequestData;
		}

		public ActionRequest GetAction() {
			return new ActionRequest(actionRequestData);
		}
	}
}
