using SoulboundEngine.Item;
using SoulboundEngine.World.Player;

#nullable enable

namespace SoulboundEngine.World.Entity {
	using Level = Level.Level;

	public class ItemEntity : Entity {
		public const int DEFAULT_PICKUP_DELAY = 0;
		public const int INFINITE_PICKUP_DELAY = int.MaxValue;
		public const int LIFETIME = 6000;
		public const int INFINITE_LIFETIME = -1;
		public const int DEFAULT_AGE = 0;
		private readonly Entity? owner;
		private ItemStack itemStack;
		private int pickupDelay = DEFAULT_PICKUP_DELAY;
		private int age = DEFAULT_AGE;

		public ItemEntity(ItemStack itemStack, Level level)
			: this(null, itemStack, level) {
		}

		public ItemEntity(Entity? owner, ItemStack itemStack, Level level)
			: base(EntityType.ITEM, level) {
			this.itemStack = itemStack;
			this.owner = owner;
		}

		protected override double GetGravity() => 0.04d;

		public override void Tick() {
			if (this.itemStack.IsEmpty()) {
				this.Destroy();
				return;
			}

			base.Tick();
			if (this.pickupDelay > 0) this.pickupDelay--;
			this.Travel();

			if (this.age != INFINITE_LIFETIME) this.age++;
			if (this.age > LIFETIME) this.Destroy();
		}

		public Entity? GetOwner() => this.owner;

		public ItemStack GetStack() => this.itemStack;
		public void SetStack(ItemStack stack) => this.itemStack = stack;

		public override void PlayerTouch(PlayerEntity player) {
			if (this.pickupDelay > 0) return;

			ItemStack remainder = player.Take(this.itemStack);
			this.SetStack(remainder);
			if (remainder.IsEmpty()) this.Destroy();
		}

		public void Destroy() {
			this.level.RemoveEntity(this);
		}
	}
}
