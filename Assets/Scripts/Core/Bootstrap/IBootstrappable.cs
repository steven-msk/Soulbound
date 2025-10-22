using System;

namespace SoulboundBackend.Core.Bootstrap {
	[Obsolete]
	public interface IBootstrappable {
		public void OnBootstrap(DependencyContainer dependencyContainer);
		public void OnEarlyBootstrap(DependencyContainer dependencyContainer);
	}
}
