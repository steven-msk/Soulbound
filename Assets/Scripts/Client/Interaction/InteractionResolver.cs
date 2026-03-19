using SoulboundBackend.Client.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoulboundBackend.Client.Interaction {
	public sealed class InteractionResolver {
		private readonly Dictionary<Type, List<object>> handlersByContext = new();

		public bool Resolve<TContext>(TContext context) where TContext : struct, IInteractionContext {
			if (!handlersByContext.TryGetValue(typeof(TContext), out List<object> boxedHandlers)) {
				Logger.LogError("Unknown interaction context type '{}'", typeof(TContext).Name);
				return false;
			}

			foreach (var handler in boxedHandlers.Cast<IInteractionHandler<TContext>>()) {
				if (handler.CanHandle(in context)) {
					return handler.Handle(in context);
				}
			}

			return false;
		}

		public void RegisterHandler<TContext>(IInteractionHandler<TContext> handler) where TContext : struct, IInteractionContext {
			Type contextType = typeof(TContext);

			if (!handlersByContext.ContainsKey(contextType)) {
				handlersByContext.Add(contextType, new List<object>());
			}
			handlersByContext[contextType].Add(handler);

			handlersByContext[contextType].Sort((x, y) => {
				IInteractionHandler<TContext> handlerX = (IInteractionHandler<TContext>)x;
				IInteractionHandler<TContext> handlerY = (IInteractionHandler<TContext>)y;
				return handlerX.priority.CompareTo(handlerY.priority);
			});
		}

	}
}
