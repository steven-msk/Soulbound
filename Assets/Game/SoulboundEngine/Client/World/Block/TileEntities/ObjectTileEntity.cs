using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.World.Level;
using SoulboundEngine.Common;
using SoulboundEngine.Common.Unity;
using SoulboundEngine.Core;
using System;
using UnityEngine;

namespace SoulboundEngine.Client.World.Block.TileEntity {
	using PlayerEntity = Player.PlayerEntity;

	[PROTOTYPICAL]
	public class ObjectTileEntity : TileEntity {
		// made to work with AreaTriggerBlock

		public event Action<PlayerEntity> onTriggerEnter;
		public event Action<PlayerEntity> onTriggerExit;
		public event Action onDestroyed;
		private readonly GameObject gameObject;

		public ObjectTileEntity(TileEntityType<ObjectTileEntity> tileEntityType, Level.Level level, BlockPos blockPos)
			: base(tileEntityType, level, blockPos) {
			this.gameObject = new GameObject("Object Tile Entity");
			this.gameObject.transform.position = blockPos.GetCenter();

			CircleCollider2D collider = this.gameObject.AddComponent<CircleCollider2D>();
			collider.isTrigger = true;
			collider.excludeLayers = ~LayerMask.GetMask(Layers.EntityCharacter);
			collider.radius = 2.5f;

			TriggerCollisionListener triggerListener = this.gameObject.AddComponent<TriggerCollisionListener>();
			triggerListener.onTriggerEnter += this.OnTriggerEnter;
			triggerListener.onTriggerExit += this.OnTriggerExit;
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
			GameObject.Destroy(this.gameObject);
		}
	}
}
