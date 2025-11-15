using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace SoulboundBackend.Core.Bootstrap {
	public abstract class AbstractInstaller : Installer {
		public abstract void InstallBindings(DiContainer container);

		public override void InstallBindings() => InstallBindings(Container);
	}

	public abstract class AbstractInstaller<T> : Installer<AbstractInstaller<T>> {
		public abstract void InstallBindinds(DiContainer container);
		public override void InstallBindings() => InstallBindinds(Container);
	}
}
