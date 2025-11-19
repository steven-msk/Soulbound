using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Core;
using System;
using UnityEngine;
using Zenject;

namespace SoulboundBackend.Core.Bootstrap {
	public class LevelInstallerComponent : MonoInstaller {
		[SerializeField] private Canvas worldCanvas;

		public override void InstallBindings() {
			new LevelInstaller(Soulbound.instance.worldManager, worldCanvas).InstallBindings(Container);
		}
	}
}