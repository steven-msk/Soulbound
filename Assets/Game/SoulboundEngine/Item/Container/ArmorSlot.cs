namespace SoulboundEngine.Item.Container {
	using SoulboundEngine.World.Entity;

	public class ArmorSlot : ItemSlot, IEquipmentSlot {
		private readonly ArmorType armorType;
		private readonly Entity owner;

		public ArmorSlot(IInventory inventory, int index, ArmorType armorType, Entity owner)
			: base(inventory, index) {
			this.armorType = armorType;
			this.owner = owner;
		}

		public override void SetStack(ItemStack stack) {
			this.owner.SetStack(this.armorType.GetSlot(), stack);
			base.SetStack(stack);
		}

		public ArmorType GetArmorType() => this.armorType;

		public EquipmentSlot GetEquipmentSlot() => this.armorType.GetSlot();
	}
}
