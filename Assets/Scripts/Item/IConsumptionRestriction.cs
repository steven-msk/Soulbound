using System.Collections.Generic;

public interface IConsumptionRestriction {
	bool CanConsume(IConsumable consumable, ItemStack itemStack);

	void NotifyConsumed(ItemStack itemStack);

	public static IEnumerable<IConsumptionRestriction> Single(IConsumptionRestriction restriction) {
		return new IConsumptionRestriction[] { restriction };
	}
}
