using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace SoulboundBackend.Client.Concurrency {
	public class ActionBinder : ConditionBinder {
		protected ActionBinder(ActionRequestData actionRequestData) 
			: base(actionRequestData) {
		}

		public ConditionBinder Execute(Action action) {
			if (action == null) {
				throw new ArgumentNullException("Action cannot be null");
			}
			actionRequestData.action = action;
			return this;
		}
	}
}
