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
		[SerializeField] SceneContext _sceneContext;
		[SerializeField] Grid _grid;
		[SerializeField] Tilemap _tilemap;
		[SerializeField] Canvas _canvas;

		public SceneContext sceneContext => _sceneContext;
		public Grid grid => _grid;
		public Tilemap tilemap => _tilemap;
		public Canvas canvas => _canvas;
	}
}
