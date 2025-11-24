using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace SoulboundBackend.Client.Concurrency {
	public class ActionBinder {
		private ActionRequestData actionRequestData = new();

		public ConditionBinder Execute(Action action) {
			if (action == null) {
				throw new ArgumentNullException("Action cannot be null");
			}
			actionRequestData.action = action;
			return new ConditionBinder(actionRequestData);
		}
	}
}
