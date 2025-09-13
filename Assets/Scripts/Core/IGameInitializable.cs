namespace SoulboundBackend.Core {
	public interface IGameInitializable<TInstance> {
		public TInstance OnGameInit();
	}
}
