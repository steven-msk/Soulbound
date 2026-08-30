namespace SoulboundEngine.World.Entity {
	using Newtonsoft.Json.Linq;
	using SoulboundEngine.Common.Patterns;
	using SoulboundEngine.Item;
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Entity.Attribute;
	using SoulboundEngine.World.Level;
	using SoulboundEngine.World.Player;
	using System;

#nullable enable

	public class ItemEntity : Entity {
		public const int DEFAULT_PICKUP_DELAY = 0;
		public const int INFINITE_PICKUP_DELAY = int.MaxValue;
		public const int LIFETIME = 6000;
		public const int INFINITE_LIFETIME = -1;
		public const int DEFAULT_AGE = 0;
		private Guid? owner;
		private ItemStack itemStack;
		private int pickupDelay = DEFAULT_PICKUP_DELAY;
		private int age = DEFAULT_AGE;

		public ItemEntity(Level level, double x, double y, ItemStack itemStack)
			: base(EntityType.ITEM, level) {
			this.SetPos(x, y);
			this.SetStack(itemStack);
			this.SetDeltaMovement(this.random.NextDouble() * 0.5d - 0.3d, 0.2d);
		}

		public ItemEntity(Level level, double x, double y, ItemStack itemStack, double deltaX, double deltaY)
			: base(EntityType.ITEM, level) {
			this.SetPos(x, y);
			this.SetStack(itemStack);
			this.SetDeltaMovement(deltaX, deltaY);
		}

		private ItemEntity(EntityDescriptor<ItemEntity> descriptor, Level level) 
			: base(descriptor, level) {
		}

		public static ItemEntity Create(EntityDescriptor<ItemEntity> descriptor, Level level) {
			return new ItemEntity(descriptor, level);
		}

		public new static AttributeSupplier.Builder CreateDefaultAttributes() {
			return Entity.CreateDefaultAttributes()
				.Add(Attributes.GRAVITY, 0.04d);
		}

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

		public Guid? GetOwner() => this.owner;

		public void SetOwner(Guid? owner) => this.owner = owner;

		public void SetOwner(Entity entity) => this.owner = entity.guid;

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

		public void SetAge(int age) => this.age = age;

		protected override void SaveAdditional(JToken json) {
			base.SaveAdditional(json);
			json["age"] = BuiltinCodecs.INT.Encode(this.age);
			json["owner"] = BuiltinCodecs.GUID.MakeOptional().Encode(OptionalExtras.OfUnmanaged(this.owner));
			json["pickupDelay"] = BuiltinCodecs.INT.Encode(this.pickupDelay);
			json["stack"] = ItemStack.EMPTY_ACCEPTING_CODEC.Encode(this.itemStack);
		}

		protected override void LoadAdditional(JObject json) {
			base.LoadAdditional(json);
			this.SetAge(BuiltinCodecs.INT.Decode(json["age"] ?? JValue.CreateNull())
				.ResultOrPartial(error => Logger.LogError("Could not load age: {}", error))
				.OrElse(0)
			);
			this.SetPickupDelay(BuiltinCodecs.INT.Decode(json["pickupDelay"] ?? JValue.CreateNull())
				.ResultOrPartial(error => Logger.LogError("Could not load pickup delay: {}", error))
				.OrElse(0)
			);
			BuiltinCodecs.GUID.MakeOptional().Decode(json["owner"] ?? JValue.CreateNull())
				.ResultOrPartial(error => Logger.LogError("Could not load owner: {}", error))
				.IfPresent(guid => guid.IfPresent(g => this.SetOwner(g)));

			ItemStack.EMPTY_ACCEPTING_CODEC.Decode(json["stack"] ?? JValue.CreateNull())
				.ResultOrPartial(error => Logger.LogError("Could not load stack: {}", error))
				.IfPresent(this.SetStack);
		}
	}
}
