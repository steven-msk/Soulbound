using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

namespace SoulboundBackend.Client.World {
	public interface IWorldSceneRoot {
		SceneContext sceneContext { get; }
		Grid grid { get; }
		Tilemap tilemap { get; }

		public LevelGridContext CreateGridContext() {
			return new LevelGridContext(grid, tilemap);
		}
	}
}
