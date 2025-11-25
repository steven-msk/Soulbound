using ModestTree;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using Zenject;

namespace SoulboundBackend.Client.Concurrency {
	public class ConcurrentActionResolver : ILateTickable {
		private static readonly Logger logger = Logger.CreateInstance();
		private readonly List<ActionRequest> requests = new();
		private readonly Dictionary<ActionToken, Func<bool>> suppressions = new();

		public void Submit(AbstractActionRequestBinder binder) {
			requests.Add(binder.GetRequest());
		}

		public void Submit(ActionRequest request) {
			requests.Add(request);
		}

		void ILateTickable.LateTick() {
			var requests = SolveConditions(this.requests);
			requests = ProcessSuppressions(requests);
			ExecuteByPriority(requests);
			this.requests.Clear();
		}

		public void ExecuteByPriority(IEnumerable<ActionRequest> requests) {
			var ordered = requests.OrderByDescending(r => r.priority).ToList();

			ActionRequest highestPriorityRequest = ordered.FirstOrDefault();
			if (highestPriorityRequest.suppressions == null /* default */) {
				return;
			}

			Execute(highestPriorityRequest);
			ordered.Remove(highestPriorityRequest);
			ordered = ProcessSuppressions(ordered).ToList();

			foreach (var request in ordered) {
				if (request.priorityType == PriorityType.NonExclusive) {
					Execute(request);
				}
			}
		}

		public void Execute(ActionRequest request) {
			this.suppressions.AddRange(request.suppressions ?? new Dictionary<ActionToken, Func<bool>>());

			if (request.action == null) {
				logger.LogWarning(null, "Caught null concurrent action executable");
			}
			request.action?.Invoke();
		}

		public IEnumerable<ActionRequest> SolveConditions(IEnumerable<ActionRequest> requests) {
			return requests.Where(r => r.conditions.All(condition => condition() == true));
		}

		public IEnumerable<ActionRequest> ProcessSuppressions(IEnumerable<ActionRequest> requests) {
			List<ActionRequest> filtered = new();

			foreach (var request in requests) {
				if (suppressions.TryGetValue(request.token, out var unsuppressCondition)) {
					if (unsuppressCondition.Invoke()) {
						suppressions.Remove(request.token);
					} else {
						continue;
					}
				}
				filtered.Add(request);
			}

			return filtered;
		}

		public bool IsSuppressed(ActionToken token) {
			return suppressions.TryGetValue(token, out var unsuppressCondition)
				&& !unsuppressCondition.Invoke();
		}
	}
}
