using SoulboundBackend.Client.Input;
using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using Zenject;

namespace SoulboundBackend.Client.Concurrency {
	public class ConcurrentActionResolver : ITickable, ILateTickable {
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

		void ITickable.Tick() {
			var unsuppressed = new List<ActionToken>();

			foreach (var (token, unsuppressCondition) in suppressions) {
				if (unsuppressCondition.Invoke()) {
					unsuppressed.Add(token);
				}
			}

			unsuppressed.ForEach(token => suppressions.Remove(token));
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
					ProcessSuppressions(ordered);
				}
			}
		}

		public void Execute(ActionRequest request) {
			if (request.suppressions != null) {
				foreach (var (token, unsuppressCondition) in request.suppressions) {
					this.suppressions.TryAdd(token, unsuppressCondition);
				}
			}

			if (request.action == null) {
				Logger.LogWarning("Caught null concurrent action executable");
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
