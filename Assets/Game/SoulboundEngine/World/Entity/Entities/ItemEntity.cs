using SoulboundEngine.Item;

#nullable enable

namespace SoulboundEngine.World.Entity {
	using Level = Level.Level;

	public class ItemEntity : Entity {
		public const float CANNOT_PICK_UP_DELAY_SEC = 2;
		private readonly Entity? owner;
		private readonly ItemStack itemStack;

		public ItemEntity(ItemStack itemStack, Level level)
			: this(null, itemStack, level) {
		}

		public ItemEntity(Entity? owner, ItemStack itemStack, Level level)
			: base(EntityType.ITEM, level) {
			this.itemStack = itemStack;
			this.owner = owner;
		}

		public Entity? GetOwner() => this.owner;
		public ItemStack GetStack() => this.itemStack;

		public void Destroy() {
			this.level.RemoveEntity(this);
		}
	}
}
