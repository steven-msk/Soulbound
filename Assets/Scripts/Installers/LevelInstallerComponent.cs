using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Core;
using System;
using Zenject;

namespace SoulboundBackend.Core.Bootstrap {
	public class LevelInstallerComponent : MonoInstaller {
		public override void InstallBindings() {
			new LevelInstaller(Soulbound.instance.worldManager).InstallBindings(Container);
		}
	}
}