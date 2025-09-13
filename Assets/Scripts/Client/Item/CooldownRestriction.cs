using SoulboundBackend.Core;
using UnityEngine;

namespace SoulboundBackend.Client.ItemSystem {
	public class CooldownRestriction : IConsumptionRestriction, IStaticResettable {
		private readonly float cooldown;
		private float lastConsumed = float.NegativeInfinity;

		public CooldownRestriction(float cooldown) {
			this.cooldown = cooldown;
			StaticResetManager.Register(this);
		}

		public bool CanConsume(IConsumable consumable, ItemStack itemStack) {
			UnityEngine.Debug.Log(lastConsumed);
			return !(Time.timeSinceLevelLoad - lastConsumed < cooldown);
		}

		public void NotifyConsumed(ItemStack itemStack) {
			lastConsumed = Time.timeSinceLevelLoad;
		}

		public void StaticReset() => lastConsumed = float.NegativeInfinity;
	}
}
