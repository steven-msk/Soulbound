using System;
using System.Collections.Generic;
using System.Linq;

namespace SoulboundBackend.Core.Event {
	public static class EventBus {
		private static bool isDispatching;
		private static readonly Queue<IGameEvent> queuedEvents = new();
		private static readonly Dictionary<Type, List<IListenerWrapper>> listenersByEventType = new();

		public static void Publish<T>(T e) where T : struct, IGameEvent {
			if (isDispatching) {
				queuedEvents.Enqueue(e);
				return;
			}
			Publish(e);
		}

		private static void Publish(IGameEvent e) {
			if (isDispatching) return;
			isDispatching = true;

			EventDispatcher dispatcher = new(e);
			dispatcher.onDispatchFinished += OnDispatchFinished;

			IEnumerable<IListenerWrapper> listeners = listenersByEventType
				.Where(kvp => kvp.Key == e.GetType())
				.SelectMany(kvp => kvp.Value)
				.ToList();
			dispatcher.Dispatch(listeners);
		}

		public static void AddListener<T>(IEventListener<T> listener) where T : struct, IGameEvent {
			Type eventType = typeof(T);
			if (!listenersByEventType.ContainsKey(eventType)) {
				listenersByEventType[eventType] = new List<IListenerWrapper>();
			}

			List<IListenerWrapper> existing = listenersByEventType[eventType];
			bool duplicate = existing.Any(l => ReferenceEquals(l.GetWrappedListener(), listener));
			if (!duplicate) existing.Add(new ListenerWrapper<T>(listener));
		}

		public static void RemoveListener<T>(IEventListener<T> listener) where T : struct, IGameEvent {
			if (listenersByEventType.TryGetValue(typeof(T), out var wrappers)) {
				wrappers.RemoveAll(l => ReferenceEquals(l.GetWrappedListener(), listener));
			}
		}

		public static void Clear() {
			listenersByEventType.Clear();
			queuedEvents.Clear();
		}

		private static void OnDispatchFinished() {
			isDispatching = false;
			if (queuedEvents.Any()) {
				Publish(queuedEvents.Dequeue());
			}
		}

		public static bool IsDispatching() => isDispatching;
	}
}
