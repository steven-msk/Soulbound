using System;

public sealed class InventoryEventHandler {
	public event Action<ArmorItem> onArmorEquipped;

	public void InvokeArmorEquipped(ArmorItem armorItem) {
		this.onArmorEquipped.Invoke(armorItem);
	}
}