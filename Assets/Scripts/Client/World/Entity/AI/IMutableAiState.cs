using System;

public interface IMutableAIState<TAI> : IAIState where TAI : IAIState {
	public void Mutate(Action<TAI> mutateAction);
}
