using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Bootstrap {
	public class Bootstrapper {
		public DependencyContainer EarlyBootstrap(IEnumerable<IBootstrappable> tree) {
			var container = new DependencyContainer();
			foreach (var bootstrappable in tree) {
				bootstrappable.OnEarlyBootstrap(container);
			}
			return container;
		}

		public void Bootstrap(IEnumerable<IBootstrappable> tree, DependencyContainer container) {
			foreach (var bootstrappable in tree) {
				bootstrappable.OnBootstrap(container);
			}
		}
	}
}
