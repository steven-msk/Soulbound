using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EventBus<TEvent> where TEvent : IEvent {
	private static readonly Dictionary<TEvent, Action> handlers = new();

	public static void Subscribe(TEvent @event, Action handler) {
		if (!handlers.ContainsKey(@event)) {
			handlers[@event] = handler;
			return;
		}
		handlers[@event] += handler;
	}

	public static void Unsubscribe(TEvent @event, Action handler) {
		if (!handlers.ContainsKey(@event)) {
			Debug.LogWarning($"Unable to unsubscribe event: missing event mapping '{@event.ID}'");
			return;
		}
		handlers[@event] -= handler;
	}

	public static void Publish(TEvent @event) { 
		if (handlers.ContainsKey(@event)) {
			handlers[@event]?.Invoke();
		}
	}

	public static void Clear() => handlers.Clear();
}