using SoulboundBackend.Client.Players;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.EntitySystem.Transform;
using SoulboundBackend.Common;
using SoulboundBackend.Common.Unity;
using SoulboundBackend.Core;
using UnityEngine;
using Logger = SoulboundBackend.Client.Debug.Logging.Logger;

namespace SoulboundBackend.Client.World.EntitySystem {
	[PROTOTYPICAL]
	public sealed class AreaTriggerEntity : Entity {
		public AreaTriggerEntity(Vector2 initialPos)
			: base(EntityType.AREA_TRIGGER_ENTITY, initialPos) {
		}

		protected override IEntityTransform CreateTransform() {
			GameObject obj = new("Area Trigger Entity");

			BoxCollider2D collider = obj.AddComponent<BoxCollider2D>();
			collider.size = new Vector2(4f, 0.15f);
			collider.isTrigger = true;
			collider.excludeLayers = ~LayerMask.GetMask(Layers.EntityCharacter);

			TriggerCollisionListener triggerListener = obj.AddComponent<TriggerCollisionListener>();
			triggerListener.onTriggerEnter += OnAreaEnter;
			triggerListener.onTriggerExit += OnAreaExit;

			return obj.AddComponent<StaticTransform>();
		}

		private void OnAreaEnter(Collider2D collider) {
			if (collider.TryGetComponent(out PlayerTransform _)) {
				Logger.LogInfo("player entered entity area");
			}
		}

		private void OnAreaExit(Collider2D collider) {
			if (collider.TryGetComponent(out PlayerTransform _)) {
				Logger.LogInfo("plyer left entity area");
			}
		}
	}
}
