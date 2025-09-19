namespace SoulboundBackend.Core {
	public interface IBootstrappable<TInstance> {
		public TInstance OnBootstrap();
	}
}
