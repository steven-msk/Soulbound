using SoulboundEngine.Client;
using SoulboundEngine.Common;
using SoulboundEngine.Common.Math;
using SoulboundEngine.Common.Unity;
using SoulboundEngine.World.Block.State;
using SoulboundEngine.World.Player;
using System;
using UnityEngine;

namespace SoulboundEngine.World.Block.Entity {

	[PROTOTYPICAL]
	public class ObjectTileEntity : TileEntity {
		// made to work with AreaTriggerBlock

		public event Action<PlayerEntity> onTriggerEnter;
		public event Action<PlayerEntity> onTriggerExit;
		public event Action onDestroyed;
		private readonly GameObject gameObject;

		public ObjectTileEntity(TileEntityType<ObjectTileEntity> tileEntityType, BlockPos blockPos, BlockState blockState)
			: base(tileEntityType, blockPos, blockState) {
			this.gameObject = new GameObject("Object Tile Entity");
			Vec2d center = blockPos.GetCenter();
			this.gameObject.transform.position = new Vector3((float)center.x, (float)center.y);

			CircleCollider2D collider = this.gameObject.AddComponent<CircleCollider2D>();
			collider.isTrigger = true;
			collider.excludeLayers = ~LayerMask.GetMask(Layers.EntityCharacter);
			collider.radius = 2.5f;

			TriggerCollisionListener triggerListener = this.gameObject.AddComponent<TriggerCollisionListener>();
			triggerListener.onTriggerEnter += this.OnTriggerEnter;
			triggerListener.onTriggerExit += this.OnTriggerExit;
		}

		public static ObjectTileEntity Create(BlockPos blockPos, BlockState blockState) {
			return new ObjectTileEntity(TileEntityType.OBJECT, blockPos, blockState);
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
