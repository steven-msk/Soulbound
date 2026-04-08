using SoulboundEngine.Client.Players;
using SoulboundEngine.Client.World.EntitySystem.Transform;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Common;
using SoulboundEngine.Common.Unity;
using SoulboundEngine.Core;
using UnityEngine;

namespace SoulboundEngine.Client.World.EntitySystem {
	using Logger = Debug.Logging.Logger;

	[PROTOTYPICAL]
	public sealed class AreaTriggerEntity : Entity {
		public AreaTriggerEntity(Level level)
			: base(EntityType.AREA_TRIGGER_ENTITY, level) {
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
