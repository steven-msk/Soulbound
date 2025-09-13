public interface IEquipable : IItemCapability {
	public void OnEquip(EquipmentSlot slot);

	public void OnUnequipped();
}