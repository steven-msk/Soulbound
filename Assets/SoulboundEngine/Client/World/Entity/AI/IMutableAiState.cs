using System;

namespace SoulboundEngine.Client.World.EntitySystem.AI {
	public interface IMutableAIState<TAI> : IAIState where TAI : IAIState {
		public void Mutate(Action<TAI> mutateAction);
	}
}
