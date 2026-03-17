using SoulboundBackend.Client.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client {
	public interface IInteractionContext {
		// TODO: find a proper use for IInteractionContext interface to avoid marking
		Level GetLevel();
		InteractionTrigger GetTrigger();
	}
}
