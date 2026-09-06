namespace SoulboundEngine.Item.Container {
	using SoulboundEngine.World.Entity;

	public interface IEquipmentSlot : IItemSlot {
		EquipmentSlot GetEquipmentSlot();
	}
}
