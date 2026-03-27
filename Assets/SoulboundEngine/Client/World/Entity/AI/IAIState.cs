namespace SoulboundEngine.Client.World.EntitySystem.AI {
	public interface IAIState {
		public abstract bool isInterruptable { get; }
		public abstract bool isFinished { get; }

		void OnEnter();
		void OnExit();
		void OnUpdate(float deltaTime);
		void Tick();
	}
}
