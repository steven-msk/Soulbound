using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public sealed class InventoryEventHandler {
	public event Action<ArmorItem> onArmorEquipped;

	public void InvokeArmorEquipped(ArmorItem armorItem) {
		this.onArmorEquipped.Invoke(armorItem);
	}
}