using System.Collections.Generic;

namespace SoulboundBackend.Client.ItemSystem {
	public interface IConsumptionRestriction {
		bool CanConsume(IConsumable consumable, ItemStack itemStack);

		void NotifyConsumed(ItemStack itemStack);

		public static IEnumerable<IConsumptionRestriction> Single(IConsumptionRestriction restriction) {
			return new IConsumptionRestriction[] { restriction };
		}
	}
}
