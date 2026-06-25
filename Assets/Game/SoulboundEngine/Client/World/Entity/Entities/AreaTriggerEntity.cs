using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.World.Level;
using SoulboundEngine.Common;
using UnityEngine;

namespace SoulboundEngine.Client.World.Entity {
	using Logger = Debug.Logging.Logger;

	[PROTOTYPICAL]
	public sealed class AreaTriggerEntity : Entity {
		public AreaTriggerEntity(EntityDescriptor<AreaTriggerEntity> descriptor, Level.Level level)
			: base(descriptor, level) {
		}

		public void OnAreaEnter(Collider2D collider) {
			if (collider.TryGetComponent(out PlayerTransform _)) {
				Logger.LogInfo("player entered entity area");
			}
		}

		public void OnAreaExit(Collider2D collider) {
			if (collider.TryGetComponent(out PlayerTransform _)) {
				Logger.LogInfo("plyer left entity area");
			}
		}
	}
}
