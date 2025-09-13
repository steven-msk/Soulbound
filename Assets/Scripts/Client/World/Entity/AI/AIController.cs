using System.Collections.Generic;

namespace SoulboundBackend.Client.World.Entity.AI {
	public class AIController : ITickable {
		public delegate IAIState StateResolver();

		private Entity self;
		private StateResolver stateResolver;
		private Queue<IAIState> stateQueue = new();
		private IAIState currentState;
		public IAIState CurrentState => currentState;

		public AIController(Entity self, StateResolver stateResolver, IAIState initialState) {
			this.self = self;
			this.stateResolver = stateResolver;
			this.currentState = initialState;
		}

		public void Tick() {
			IAIState state = stateResolver.Invoke();
			if (state != currentState) {
				if (currentState.isInterruptable || currentState.isFinished) {
					SetState(state);
				} else if (!stateQueue.Contains(state)) {
					stateQueue.Enqueue(state);
				}
			}
			currentState.Tick();

			if (currentState.isFinished && stateQueue.Count > 0) {
				SetState(stateQueue.Dequeue());
			}
		}

		public void UpdateCurrentState(float deltaTime) {
			currentState.OnUpdate(deltaTime);
		}

		public void SetState(IAIState state) {
			currentState?.OnExit();
			currentState = state;
			currentState.OnEnter();
		}
	}
}
