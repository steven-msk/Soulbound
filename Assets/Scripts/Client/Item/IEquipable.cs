using SoulboundBackend.Client.UI.Storage;

namespace SoulboundBackend.Client.ItemSystem {
	public interface IEquipable : IItemCapability {
		public void OnEquip(EquipmentSlot slot);

		public void OnUnequipped();
	}
}