using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class EventPriorityManager {
	public static int HighestPriority { get; private set; } = 0;
	private static readonly List<PrioritizedEvent> eventStack = new();

	public static void RequestControl(PrioritizedEvent @event, bool logClaim = false) {
		static void PushEvent(PrioritizedEvent @event, int priority, bool logClaim) {
			eventStack.Add(@event);
			if (logClaim) {
				Debug.Log($"Event context control request success: {@event}, priority: {priority}");
			}
		}

		if (eventStack.Count == 0) {
			PushEvent(@event, @event.Priority, logClaim);
			return;
		}
		
		var currentTop = eventStack.Last();
		if (@event.Priority > currentTop.Priority) {
			PushEvent(@event, @event.Priority, logClaim);
			return;
		} else {
			if (currentTop.Allows?.Contains(@event.Context) ?? false) {
				return;
			}
		}
		throw new PriorityRequestException(@event, currentTop);
	}

	public static void Revoke(PrioritizedEvent @event, bool logRelease = false) {
			int index = eventStack.FindIndex(activeEvent => activeEvent == @event);
			if (index != -1) {
				eventStack.RemoveAt(index);
				if (logRelease) {
					Debug.Log($"Event released: {@event}");
				}
			}
	}

	public static void Revoke(string context, bool logRelease = false) {
			int index = eventStack.FindIndex(activeEvent => activeEvent.Context.Equals(context));
			if (index != -1) {
				eventStack.RemoveAt(index);
				if (logRelease) {
					Debug.Log($"Event context released: {context}");
				}
			}
	}

	public static bool IsAllowed(PrioritizedEvent @event) {
		if (eventStack.Count > 0) {
			return eventStack.Last().AllowsContext(@event);
		}
		return true;
	}

	public static bool IsAllowed(string context) {
		if (eventStack.Count > 0) {
			return eventStack.Last().AllowsContext(context); 
		}
		return true;
	}
}

public class PriorityRequestException : Exception {
	public PrioritizedEvent Attempted { get; }
	public PrioritizedEvent Current { get; }

	public PriorityRequestException(PrioritizedEvent attempted, PrioritizedEvent current)
		: base($"Event context control request failed. Tried {attempted}, but {current} is already active.") {
		Attempted = attempted;
		Current = current;
	}
}
