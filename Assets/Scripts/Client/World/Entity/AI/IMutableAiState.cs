using System;

namespace SoulboundBackend.Client.World.EntitySystem.AI {
	public interface IMutableAIState<TAI> : IAIState where TAI : IAIState {
		public void Mutate(Action<TAI> mutateAction);
	}
}
