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
		private Entity? owner;
		private ItemStack itemStack;
		private int pickupDelay = DEFAULT_PICKUP_DELAY;
		private int age = DEFAULT_AGE;

		public ItemEntity(Level level, double x, double y, ItemStack itemStack)
			: base(EntityType.ITEM, level) {
			this.SetPos(x, y);
			this.SetStack(itemStack);
			this.SetDeltaMovement(this.random.NextDouble() * 0.2d - 0.1d, 0.2d);
		}

		public ItemEntity(Level level, double x, double y, ItemStack itemStack, double deltaX, double deltaY)
			: base(EntityType.ITEM, level) {
			this.SetPos(x, y);
			this.SetStack(itemStack);
			this.SetDeltaMovement(deltaX, deltaY);
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
		public void SetOwner(Entity owner) => this.owner = owner;

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

		public void SetPickupDelay(int delay) => this.pickupDelay = delay;
	}
}
