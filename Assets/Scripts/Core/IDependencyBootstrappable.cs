namespace SoulboundBackend.Core {
	public interface IDependencyBootstrappable<TInstance, TDependency> {
		public TInstance OnBootstrap(TDependency dependency);
	}
}
