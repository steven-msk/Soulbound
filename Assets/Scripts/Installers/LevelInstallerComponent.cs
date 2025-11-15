using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Core;
using System;
using UnityEngine;
using Zenject;

namespace SoulboundBackend.Core.Bootstrap {
	public class LevelInstallerComponent : MonoInstaller {
		public override void InstallBindings() {
			new LevelInstaller(Soulbound.instance.worldManager, GameObject.Find("Canvas").GetComponent<Canvas>()).InstallBindings(Container);
		}
	}
}