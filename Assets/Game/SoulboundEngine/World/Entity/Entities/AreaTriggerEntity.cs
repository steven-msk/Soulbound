using SoulboundEngine.Client.Player;
using SoulboundEngine.Common;
using UnityEngine;
using Logger = SoulboundEngine.Client.Debug.Logging.Logger;

namespace SoulboundEngine.World.Entity {
	using Level = Level.Level;

	[PROTOTYPICAL]
	public sealed class AreaTriggerEntity : Entity {
		public AreaTriggerEntity(EntityDescriptor<AreaTriggerEntity> descriptor, Level level)
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
