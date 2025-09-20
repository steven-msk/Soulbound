namespace SoulboundBackend.Core.Bootstrap {
	public interface IBootstrappable {
		public void OnBootstrap(DependencyContainer dependencyContainer);
		public void OnEarlyBootstrap(DependencyContainer dependencyContainer);
	}
}
