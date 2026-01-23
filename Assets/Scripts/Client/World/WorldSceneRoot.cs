using SoulboundBackend.Core.Bootstrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

namespace SoulboundBackend.Client.World {
	public sealed class WorldSceneRoot : MonoBehaviour, IWorldSceneRoot {
		public SceneContext sceneContext => GetComponent<SceneContext>();
		public Grid grid => FindFirstObjectByType<Grid>();
		public Tilemap tilemap => FindFirstObjectByType<Tilemap>();
	}
}
