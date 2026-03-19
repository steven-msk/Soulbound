using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Interaction {
	public interface IInteractionHandler<TContext> where TContext : struct, IInteractionContext {
		int priority { get; }
		bool CanHandle(in TContext ctx);
		bool Handle(in TContext ctx);
	}
}
