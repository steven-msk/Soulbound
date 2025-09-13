using System;

namespace SoulboundBackend.Client.World.Entity.AI {
	public interface IMutableAIState<TAI> : IAIState where TAI : IAIState {
		public void Mutate(Action<TAI> mutateAction);
	}
}
