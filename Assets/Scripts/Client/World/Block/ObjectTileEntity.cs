using SoulboundBackend.Common;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.World.BlockSystem {
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
