using SoulboundBackend.Core.Bootstrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace SoulboundBackend.Client.World {
	public class LevelInstallerComponent : MonoInstaller {
		[SerializeField] WorldSceneRoot sceneRoot;

		public override void InstallBindings() {
			new LevelInstaller(sceneRoot.canvas).InstallBindings(Container);
		}
	}
}
