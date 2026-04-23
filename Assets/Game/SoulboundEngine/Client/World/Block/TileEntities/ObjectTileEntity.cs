using SoulboundEngine.Client.Players;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Common;
using SoulboundEngine.Common.Unity;
using SoulboundEngine.Core;
using System;
using UnityEngine;

namespace SoulboundEngine.Client.World.BlockSystem.TileEntities {
	[PROTOTYPICAL]
	public class ObjectTileEntity : TileEntity {
		// made to work with AreaTriggerBlock

		public event Action<Player> onTriggerEnter;
		public event Action<Player> onTriggerExit;
		public event Action onDestroyed;
		private readonly GameObject gameObject;

		public ObjectTileEntity(Level level, BlockPos blockPos)
			: base(level, blockPos) {
			gameObject = new GameObject("Object Tile Entity");
			gameObject.transform.position = blockPos.GetCenter();

			CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
			collider.isTrigger = true;
			collider.excludeLayers = ~LayerMask.GetMask(Layers.EntityCharacter);
			collider.radius = 2.5f;

			TriggerCollisionListener triggerListener = gameObject.AddComponent<TriggerCollisionListener>();
			triggerListener.onTriggerEnter += OnTriggerEnter;
			triggerListener.onTriggerExit += OnTriggerExit;
		}

		private void OnTriggerEnter(Collider2D collider) {
			if (collider.TryGetComponent<PlayerTransform>(out var playerTransform)) {
				onTriggerEnter?.Invoke(playerTransform.GetEntity());
			}
		}

		private void OnTriggerExit(Collider2D collider) {
			if (collider.TryGetComponent<PlayerTransform>(out var playerTransform)) {
				onTriggerExit?.Invoke(playerTransform.GetEntity());
			}
		}

		public override void OnDispose() {
			onDestroyed?.Invoke();
			GameObject.Destroy(gameObject);
		}
	}
}
