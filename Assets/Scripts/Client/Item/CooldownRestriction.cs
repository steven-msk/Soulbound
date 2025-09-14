using SoulboundBackend.Core;
using UnityEngine;

namespace SoulboundBackend.Client.ItemSystem {
	public class CooldownRestriction : IConsumptionRestriction {
		private readonly float cooldown;
		private float activeUntil;

		public CooldownRestriction(float cooldown) {
			this.cooldown = cooldown;
		}

		ConsumptionDirective IConsumptionRestriction.Evaluate(IConsumable consumable, ItemStack itemStack) {
			if (Time.time < activeUntil) {
				return new ConsumptionDirective(ConsumeMode.Override, itemStack => ResetTimer());
			}
			return new ConsumptionDirective(ConsumeMode.Allow);
		}

		void IConsumptionRestriction.NotifyConsumed(ItemStack itemStack) => ResetTimer();

		public void ResetTimer() {
			UnityEngine.Debug.Log("resetting timer");
			activeUntil = Time.time + cooldown;
		}
	}
}
