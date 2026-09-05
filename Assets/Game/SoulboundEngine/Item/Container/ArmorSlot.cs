namespace SoulboundEngine.Item.Container {
	using SoulboundEngine.World.Entity;

	public class ArmorSlot : ItemSlot {
		private readonly EquipmentSlot equipmentSlot;
		private readonly Entity owner;

		public ArmorSlot(IInventory inventory, int index, EquipmentSlot equipmentSlot, Entity owner) 
			: base(inventory, index) {
			this.equipmentSlot = equipmentSlot;
			this.owner = owner;
		}

		public override void SetStack(ItemStack stack) {
			this.owner.SetStack(this.equipmentSlot, stack);
			base.SetStack(stack);
		}
	}
}
