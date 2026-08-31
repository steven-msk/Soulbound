namespace SoulboundEngine.World.Entity {
	using SoulboundEngine.Item;
	using System.Collections.Generic;

	public class EntityEquipment {
		private readonly Dictionary<EquipmentSlot, ItemStack> items = new();

		private EntityEquipment(Dictionary<EquipmentSlot, ItemStack> items) {
			this.items = items;
		}

		public EntityEquipment()
			: this(new Dictionary<EquipmentSlot, ItemStack>()) {
		}

		public virtual ItemStack Get(EquipmentSlot slot) => this.items.GetValueOrDefault(slot, ItemStack.EMPTY);

		public virtual ItemStack Set(EquipmentSlot slot, ItemStack itemStack) {
			ItemStack old = this.Get(slot);
			this.items[slot] = itemStack;
			return old;
		}

		public virtual bool IsEmpty() {
			foreach (ItemStack stack in this.items.Values) {
				if (!stack.IsEmpty()) return false;
			}
			return true;
		}
		
		public virtual void Tick(Entity owner) {
			foreach ((EquipmentSlot slot, ItemStack stack) in this.items) {
				if (!stack.IsEmpty()) {
					stack.InventoryTick(owner.GetLevel(), owner, slot);
				}
			}
		}

		public void SetFrom(EntityEquipment equipment) {
			this.items.Clear();
			foreach ((EquipmentSlot slot, ItemStack stack) in equipment.items) {
				this.items.Add(slot, stack);
			}
		}

		public virtual void DropAll(Entity dropper) {
			foreach (ItemStack stack in this.items.Values) {
				dropper.DropStack(stack);
			}
			this.Clear();
		}

		public void Clear() {
			foreach (EquipmentSlot slot in this.items.Keys) {
				this.items[slot] = ItemStack.EMPTY;
			}
		}
	}
}
