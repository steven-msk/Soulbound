using System;

namespace SoulboundEngine.Client.Interaction {
	[Obsolete]
	public interface IInteractionHandler<TContext> where TContext : struct, IInteractionContext {
		int priority { get; }
		bool CanHandle(in TContext ctx);
		bool Handle(in TContext ctx);
	}
}
