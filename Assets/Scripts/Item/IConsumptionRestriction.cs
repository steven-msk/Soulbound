using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IConsumptionRestriction {
	bool CanConsume(IConsumable consumable, ItemStack itemStack);

	void NotifyConsumed(ItemStack itemStack);

	public static IEnumerable<IConsumptionRestriction> Single(IConsumptionRestriction restriction) {
		return new IConsumptionRestriction[] { restriction };
	}
}
