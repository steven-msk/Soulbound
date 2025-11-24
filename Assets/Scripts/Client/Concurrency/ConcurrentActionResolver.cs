using SoulboundBackend.Client.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace SoulboundBackend.Client.Concurrency {
	public class ConcurrentActionResolver : ITickable, ILateTickable {
		private readonly List<ActionRequest> requests = new();

		public void Submit(AbstractActionRequestBinder binder) {
			requests.Add(binder.GetAction());
		}

		public void Submit(ActionRequest request) {
			requests.Add(request);
		}

		void ILateTickable.LateTick() {
			var conditionsPassed = SolveConditions(this.requests);
			ExecuteByPriority(conditionsPassed);
			requests.Clear();
		}

		public void ExecuteByPriority(IEnumerable<ActionRequest> requests) {
			var ordered = requests.OrderByDescending(r => r.priority).ToList();

			ActionRequest highestPriorityRequest = ordered.FirstOrDefault();
			highestPriorityRequest.action?.Invoke();
			ordered.Remove(highestPriorityRequest);

			foreach (var request in ordered) {
				if (request.priorityType == PriorityType.NonExclusive) {
					request.action.Invoke();
				}
			}
		}

		public IEnumerable<ActionRequest> SolveConditions(IEnumerable<ActionRequest> requests) {
			return requests.Where(r => r.conditions.All(condition => condition() == true));
		}

		void ITickable.Tick() {
			//List<string> unblockedPersistent = new();
			//foreach (var kvp in blockedContexts) {
			//	if (kvp.Value.Invoke()) {
			//		unblockedPersistent.Add(kvp.Key);
			//	}
			//}
			//unblockedPersistent.ForEach(context => blockedContexts.Remove(context));
		}
	}
}
