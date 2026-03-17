using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.BlockSystem {
	public interface IBlockInteractionListener {
		bool CanInteract(in BlockInteraction ctx);
		bool ValidateTrigger(InteractionTrigger trigger);
		void OnInteract(in BlockInteraction ctx);
	}
}
