namespace SoulboundEngine.World.Entity {
	using Newtonsoft.Json.Linq;
	using SoulboundEngine.Item;
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
			this.SetDeltaMovement(this.random.NextDouble() * 0.2d - 0.1d, 0.2d);
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

		protected override void SaveAdditional(JObject json) {
			base.SaveAdditional(json);
			json["age"] = this.age;
			json["owner"] = this.owner == null ? JValue.CreateNull() : this.owner;
			json["pickupDelay"] = this.pickupDelay;
			json["stack"] = ItemStack.ToJson(this.itemStack);
		}

		protected override void LoadAdditional(JObject json) {
			base.LoadAdditional(json);
			int? age = (int?)json["age"];
			if (age == null) Logger.LogError("No age property on ItemEntity json: {}", json);

			int? pickupDelay = (int?)json["pickupDelay"];
			if (pickupDelay == null) Logger.LogError("No pickupDelay property on ItemEntity json: {}", json);

			JToken? stackToken = json["stack"];
			if (stackToken == null) Logger.LogError("No stack property on ItemEntity json: {}", json);

			JProperty? ownerProperty = json.Property("owner");
			Guid? guid = null;
			if (ownerProperty == null) {
				Logger.LogError("No owner property on ItemEntity json: {}", json);
			} else {
				string? guidString = (string?)ownerProperty.Value;
				if (guidString != null) {
					if (Guid.TryParse(guidString, out Guid value)) {
						guid = value;
					} else {
						Logger.LogError("Could not parse ItemEntity guid: {}", guidString);
					}
				}
			}

			this.SetOwner(guid);
			this.SetAge(age.GetValueOrDefault(0));
			this.SetPickupDelay(pickupDelay.GetValueOrDefault(0));
			this.SetStack(stackToken != null ? ItemStack.FromJson(stackToken) : ItemStack.EMPTY);
		}
	}
}
