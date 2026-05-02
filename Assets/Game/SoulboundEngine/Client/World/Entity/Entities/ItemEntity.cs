using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.World.LevelDomain;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem {
	public class ItemEntity : Entity {
		public const float CANNOT_PICK_UP_DELAY_SEC = 2;
		public static readonly EntityDescriptor<ItemEntity> DESCRIPTOR = EntityDescriptor.Of<ItemEntity>((_, level) => new ItemEntity(null, level));

		private readonly Entity? owner;
		private readonly ItemStack itemStack;

		public ItemEntity(ItemStack itemStack, Level level)
			: this(null, itemStack, level) {
		}

		public ItemEntity(Entity? owner, ItemStack itemStack, Level level)
			: base(DESCRIPTOR, level) {
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
