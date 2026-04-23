using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable enable

namespace SoulboundEngine.Core.Event {
	public static class EventBus {
		private static bool isDispatching;
		private static readonly Queue<(IGameEvent, Action<HashSet<Type>>?)> queuedEvents = new();
		private static readonly Dictionary<Type, List<IListenerWrapper>> listenersByEventType = new();
		private static readonly Dictionary<Type, List<IHandlerWrapper>> handlersByEventType = new();
		private static readonly Dictionary<Type, EventSubscriptionMetadata> metadataCache = new();
		
		public static void Publish<T>(T e, Action<HashSet<Type>>? response = null) where T : struct, IGameEvent {
			if (isDispatching) {
				queuedEvents.Enqueue((e, response));
				return;
			}
			Publish((IGameEvent)e, response);
		}

		private static void Publish(IGameEvent e, Action<HashSet<Type>>? response) {
			if (isDispatching) return;
			isDispatching = true;

			HashSet<Type> responseTypes = new();
			if (handlersByEventType.TryGetValue(e.GetType(), out var handlers)) {
				foreach (var handler in handlers.ToArray()) {
					handler.Fire(e);
					responseTypes.Add(handler.GetHandlerType());
				}
			}
			if (listenersByEventType.TryGetValue(e.GetType(), out var listeners)) {
				foreach (var listener in listeners.ToArray()) {
					listener.Fire(e);
				}
			}

			OnDispatchFinished();
			response?.Invoke(responseTypes);
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

		public static void AddHandler<T>(IEventHandler<T> handler) where T : struct, IGameEvent {
			Type eventType = typeof(T);
			if (!handlersByEventType.ContainsKey(eventType)) {
				handlersByEventType[eventType] = new List<IHandlerWrapper>();
			}

			List<IHandlerWrapper> existing = handlersByEventType[eventType];
			bool duplicate = existing.Any(h => ReferenceEquals(h.GetWrappedListener(), handler));
			if (!duplicate) existing.Add(new HandlerWrapper<T>(handler));
		}

		public static void RemoveHandler<T>(IEventHandler<T> handler) where T : struct, IGameEvent {
			if (handlersByEventType.TryGetValue(typeof(T), out var wrappers)) {
				wrappers.RemoveAll(h => ReferenceEquals(h.GetWrappedListener(), handler));
			}
		}

		public static void AddAllInterfaces(object obj) {
			Type type = obj.GetType();
			if (!metadataCache.TryGetValue(type, out var meta)) {
				meta = BuildMetadata(type);
				metadataCache[type] = meta;
			}
			meta.add(obj);
		}

		public static void RemoveAllInterfaces(object obj) {
			Type type = obj.GetType();
			if (!metadataCache.TryGetValue(type, out var meta)) {
				meta = BuildMetadata(type);
				metadataCache[type] = meta;
			}
			meta.remove(obj);
		}

		private static EventSubscriptionMetadata BuildMetadata(Type type) {
			Type[] interfaces = type.GetInterfaces();
			List<Action<object>> addActions = new();
			List<Action<object>> removeActions = new();

			foreach (var iface in interfaces) {
				if (!iface.IsGenericType) continue;

				Type definition = iface.GetGenericTypeDefinition();
				Type eventType = iface.GetGenericArguments()[0];

				if (definition == typeof(IEventListener<>)) {
					MethodInfo addMethod = typeof(EventBus)
						.GetMethod(nameof(AddListener))
						.MakeGenericMethod(eventType);

					addActions.Add(obj => addMethod.Invoke(null, new[] { obj }));

					MethodInfo removeMethod = typeof(EventBus)
						.GetMethod(nameof(RemoveListener))
						.MakeGenericMethod(eventType);

					removeActions.Add(obj => removeMethod.Invoke(null, new[] { obj }));
				}

				if (definition == typeof(IEventHandler<>)) {
					MethodInfo addMethod = typeof(EventBus)
						.GetMethod(nameof(AddHandler))
						.MakeGenericMethod(eventType);

					addActions.Add(obj => addMethod.Invoke(null, new[] { obj }));

					MethodInfo removeMethod = typeof(EventBus)
						.GetMethod(nameof(RemoveHandler))
						.MakeGenericMethod(eventType);

					removeActions.Add(obj => removeMethod.Invoke(null, new[] { obj }));
				}
			}

			return new EventSubscriptionMetadata {
				add = obj => {
					foreach (var action in addActions) action(obj);
				},
				remove = obj => {
					foreach (var action in removeActions) action(obj);
				}
			};
		}

		public static void Clear() {
			listenersByEventType.Clear();
			handlersByEventType.Clear();
			queuedEvents.Clear();
			metadataCache.Clear();
		}

		private static void OnDispatchFinished() {
			isDispatching = false;
			if (queuedEvents.Any()) {
				(IGameEvent e, Action<HashSet<Type>>? response) = queuedEvents.Dequeue();
				Publish(e, response);
			}
		}

		public static bool IsDispatching() => isDispatching;
	}
}
