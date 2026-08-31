namespace SoulboundEngine.World.Entity {
	using Newtonsoft.Json.Linq;
	using SoulboundEngine.Common;
	using SoulboundEngine.Common.Collection;
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.Common.Math.Random;
	using SoulboundEngine.Item;
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Block.State;
	using SoulboundEngine.World.Chunk;
	using SoulboundEngine.World.Entity.Attribute;
	using SoulboundEngine.World.Level;
	using SoulboundEngine.World.Physics;
	using SoulboundEngine.World.Player;
	using System;
	using System.Collections.Generic;

#nullable enable

	public abstract class Entity {
		public const double DEFAULT_BB_WIDTH = 1.0d;
		public const double DEFAULT_BB_HEIGHT = 2.0d;
		public const float DEFAULT_BLOCK_FRICTION = 0.6f;
		public const float CONSTANT_DECELERATION = 0.91f;
		public static readonly Predicate<Entity> CAN_BE_COLLIDED_WITH = e => e.CanBeCollidedWith(null);
		public static readonly Predicate<Entity> ALL = _ => true;
		private readonly EntityDescriptor descriptor;
		private readonly EntityDimensions dimensions;
		protected readonly IRandom random = RandomProvider.CreateWithUniqueSeed();
		private readonly AttributeMap attributes;
		private readonly EntityEquipment equipment;
		private readonly Dictionary<EquipmentSlot, ItemStack> lastEquipmentStacks = Collections.Dictionary(
			() => EquipmentSlot.VALUES, _ => ItemStack.EMPTY
		);
		protected Level level;
		protected bool isAlive;
		protected bool firstTick = true;
		private BlockState? inBlockState;
		private Vec2d? lastKnownPos;
		private Vec2d lastKnownSpeed = Vec2d.ZERO;
		public bool noPhysics;
		public bool horizontalCollision;
		public bool verticalCollision;
		public bool verticalCollisionBelow;
		private Vec2d deltaMovement;
		private bool isOnGround;

		protected Entity(EntityDescriptor descriptor, Level level) {
			this.descriptor = descriptor;
			this.level = level;
			this.dimensions = descriptor.GetDimensions();
			this.attributes = new AttributeMap(descriptor.GetAttributes());
			this.equipment = this.CreateEquipment();
			this.SetPos(0.0d, 0.0d);
		}

		public static AttributeSupplier.Builder CreateDefaultAttributes() {
			return AttributeSupplier.Create()
				.Add(Attributes.SPEED)
				.Add(Attributes.GRAVITY);
		}

		public Vec2d position { get; private set; } = Vec2d.ZERO;
		public AABB boundingBox { get; private set; }
		public Facing facing { get; private set; } = Facing.RIGHT;
		public Guid guid { get; private set; }
		public BlockPos blockPosition { get; private set; }
		public ChunkPos chunkPosition { get; private set; }

		public void SetGuid(Guid guid) => this.guid = guid;

		public void OnAdd(Guid guid) {
			if (this.IsAlive()) throw new InvalidOperationException($"Entity already added: {guid}");

			this.guid = guid;
			this.SetAlive(true);
		}

		public virtual void Tick() {
			this.DefaultTick();
		}

		public void DefaultTick() {
			this.CalculateSpeed();
			this.inBlockState = null;
			this.firstTick = false;

			this.DetectAndHandleEquipmentChanges();
			this.equipment.Tick(this);
		}

		protected void CalculateSpeed() {
			this.lastKnownPos ??= this.position;
			this.lastKnownSpeed = this.GetPosition().Subtract(this.lastKnownPos.Value);
			this.lastKnownPos = this.GetPosition();
		}

		public Level GetLevel() => this.level;

		public void Dispose() {
			this.OnDisposed();
			this.SetAlive(false);
		}

		public bool IsAlive() => this.isAlive;
		public void SetAlive(bool alive) => this.isAlive = alive;

		protected virtual void OnDisposed() {
		}

		public EntityDescriptor GetDescriptor() => this.descriptor;

		public ItemEntity DropItem(IItemConvertible item) {
			return this.DropStack(item.AsItem().GetDefaultStack(1));
		}

		public ItemEntity DropStack(ItemStack stack) {
			Vec2d pos = this.GetPosition();
			ItemEntity entity = new(this.level, pos.x, pos.y, stack);
			entity.SetOwner(this);
			this.level.AddNewEntity(entity);
			return entity;
		}

		public void SetPos(Vec2d pos) {
			this.SetPos(pos.x, pos.y);
		}

		public void SetPos(double x, double y) {
			this.SetPosRaw(x, y);
			this.SetBoundingBox(this.MakeBoundingBox());
		}

		public void SetPosRaw(double x, double y) {
			if (this.position.x == x && this.position.y == y) return;
			this.position = new Vec2d(x, y);

			int fx = Maths.FloorToInt(x);
			int fy = Maths.FloorToInt(y);
			if (fx != this.blockPosition.x || fy != this.blockPosition.y) {
				this.blockPosition = new BlockPos(fx, fy);
				this.inBlockState = null;
				if (SectionPos.BlockToSectionCoord(fx) != this.chunkPosition.x) {
					this.chunkPosition = ChunkPos.Containing(this.blockPosition);
				}
			}
		}

		public void Travel() => this.Travel(Vec2d.ZERO);

		public void Travel(Vec2d input) {
			this.MoveRelative(this.GetFrictionInfluencedSpeed(DEFAULT_BLOCK_FRICTION), input);
			this.Move(this.GetDeltaMovement());

			Vec2d delta = this.GetDeltaMovement();
			delta.y -= this.GetAppliedGravity();
			float friction = DEFAULT_BLOCK_FRICTION * CONSTANT_DECELERATION;
			this.SetDeltaMovement(delta.x * friction, delta.y);
		}

		private float GetFrictionInfluencedSpeed(float friction) {
			return this.GetSpeed() * (0.21600002f / (friction * friction * friction));
		}

		public void MoveRelative(float speed, Vec2d input) {
			Vec2d delta = GetInputVector(input, speed);
			this.SetDeltaMovement(this.GetDeltaMovement().Add(delta));
		}

		protected static Vec2d GetInputVector(Vec2d input, float speed) {
			double length = input.lengthSqr;
			if (length < 1.0E-7) return Vec2d.ZERO;

			Vec2d movement = (length > 1.0d ? input.Normalize() : input).Multiply(speed);
			return movement;
		}

		public void ApplyGravity() {
			double gravity = this.GetAppliedGravity();
			if (gravity == 0.0d) return;
			this.SetDeltaMovement(this.GetDeltaMovement().Add(0.0d, -gravity));
		}

		public void Move(Vec2d delta) {
			if (this.noPhysics) {
				this.SetPos(this.position.x + delta.x, this.position.y + delta.y);
				this.horizontalCollision = false;
				this.verticalCollision = false;
				this.verticalCollisionBelow = false;
				return;
			}

			Vec2d movement = this.Collide(delta);
			double movementLengthSqr = movement.lengthSqr;

			if (movementLengthSqr > 1.0E-7 || delta.lengthSqr - movementLengthSqr < 1.0E-7) {
				Vec2d newPosition = this.position.Add(movement);
				this.SetPos(newPosition);
			}

			bool xCollision = !Maths.AreEqual(delta.x, movement.x);
			bool yCollision = !Maths.AreEqual(delta.y, movement.y);
			this.horizontalCollision = xCollision;
			this.verticalCollision = yCollision;
			this.verticalCollisionBelow = yCollision && delta.y < 0.0d;
			this.isOnGround = this.verticalCollisionBelow;

			Vec2d dm = this.deltaMovement;
			this.deltaMovement = new Vec2d(xCollision ? 0.0d : dm.x, yCollision ? 0.0d : dm.y);
		}

		private Vec2d Collide(Vec2d movement) {
			if (movement.lengthSqr < 1.0E-7) return movement;

			AABB box = this.boundingBox;
			List<AABB> colliders = CollectColliders(this, this.level, box.ExpandBy(movement));
			return colliders.Count == 0 ? movement : CollideWithShapes(movement, box, colliders);
		}

		public static List<AABB> CollectColliders(Entity? entity, Level level, AABB box) {
			List<AABB> list = new();
			list.AddRange(level.GetBlockCollisionBoxes(box));
			list.AddRange(level.GetEntityCollisions(entity, box));
			return list;
		}

		private static Vec2d CollideWithShapes(Vec2d movement, AABB box, IEnumerable<AABB> colliders) {
			double resolvedX, resolvedY;
			if (Math.Abs(movement.x) >= Math.Abs(movement.y)) {
				resolvedX = ClampAxis(Axis.X, box, colliders, movement.x);
				resolvedY = ClampAxis(Axis.Y, box.Move(resolvedX, 0.0d), colliders, movement.y);
			} else {
				resolvedY = ClampAxis(Axis.Y, box, colliders, movement.y);
				resolvedX = ClampAxis(Axis.X, box.Move(0.0d, resolvedY), colliders, movement.x);
			}
			return new Vec2d(resolvedX, resolvedY);
		}

		private static double ClampAxis(Axis axis, AABB box, IEnumerable<AABB> colliders, double delta) {
			if (Math.Abs(delta) < 1.0E-4) return 0.0d;

			foreach (AABB collider in colliders) {
				if (!box.OverlapsOnOtherAxis(axis, collider)) continue;
				delta = axis.Is(Axis.X) ? ClampX(box, collider, delta) : ClampY(box, collider, delta);
			}
			return delta;
		}

		private static double ClampX(AABB box, AABB collider, double delta) {
			if (delta > 0.0 && box.maxX <= collider.minX) {
				delta = Math.Min(delta, collider.minX - box.maxX);
			} else if (delta < 0.0 && box.minX >= collider.maxX) {
				delta = Math.Max(delta, collider.maxX - box.minX);
			}
			return delta;
		}

		private static double ClampY(AABB box, AABB collider, double delta) {
			if (delta > 0.0 && box.maxY <= collider.minY) {
				delta = Math.Min(delta, collider.minY - box.maxY);
			} else if (delta < 0.0 && box.minY >= collider.maxY) {
				delta = Math.Max(delta, collider.maxY - box.minY);
			}
			return delta;
		}

		public virtual void PlayerTouch(PlayerEntity player) {
		}

		public BlockState GetInBlockState() {
			return this.inBlockState ??= this.level.GetBlockState(this.blockPosition);
		}

		public virtual bool CanCollideWith(Entity entity) {
			return entity.CanBeCollidedWith(this);
		}

		public virtual bool CanBeCollidedWith(Entity? other) {
			return false;
		}

		public bool IsOnGround() => this.isOnGround;
		public void SetOnGround(bool onGround) => this.isOnGround = onGround;

		public void SetBoundingBox(AABB box) {
			this.boundingBox = box;
		}

		protected AABB MakeBoundingBox() {
			return this.MakeBoundingBox(this.position);
		}

		protected virtual AABB MakeBoundingBox(Vec2d position) {
			return this.dimensions.MakeBoundingBox(position);
		}

		public EntityDimensions GetDimensions() => this.dimensions;

		public EntityDimensions GetDefaultDimensions() => this.descriptor.GetDimensions();

		public double GetBBWidth() => this.dimensions.width;
		public double GetBBHeight() => this.dimensions.height;

		public Vec2d GetKnownSpeed() {
			return this.lastKnownSpeed;
		}

		public virtual float GetSpeed() => (float)this.GetAttributeValue(Attributes.SPEED);

		protected virtual double GetGravity() => this.GetAttributeValue(Attributes.GRAVITY);

		protected double GetAppliedGravity() {
			return this.GetGravity();
		}

		public Vec2d GetDeltaMovement() => this.deltaMovement;
		public void SetDeltaMovement(Vec2d v) {
			if (v.IsFinite) this.deltaMovement = v;
		}
		public void SetDeltaMovement(double x, double y) {
			this.deltaMovement = new Vec2d(x, y);
		}

		public void ReapplyPosition() {
			this.lastKnownPos = null;
			this.SetPos(this.position.x, this.position.y);
		}

		public Vec2d GetPosition() => this.position;
		public double GetX() => this.position.x;
		public double GetY() => this.position.y;

		public void SetFacing(Facing facing) {
			this.facing = facing;
		}

		public AttributeMap GetAttributes() => this.attributes;

		public double GetAttributeValue(RegistryEntry<AttributeType> attribute) {
			return this.GetAttributes().GetValue(attribute);
		}

		public AttributeInstance? GetAttributeInstance(RegistryEntry<AttributeType> attribute) {
			return this.GetAttributes().GetInstance(attribute);
		}

		public double GetAttributeBaseValue(RegistryEntry<AttributeType> attribute) {
			return this.GetAttributes().GetBaseValue(attribute);
		}

		protected virtual EntityEquipment CreateEquipment() {
			return new EntityEquipment();
		}

		private void DetectAndHandleEquipmentChanges() {
			Dictionary<EquipmentSlot, ItemStack>? changedItems = this.CollectEquipmentChanges();
			if (changedItems != null) this.HandleEquipmentChanges(changedItems);
		}

		private void HandleEquipmentChanges(Dictionary<EquipmentSlot, ItemStack> changedItems) {
			foreach ((EquipmentSlot slot, ItemStack current) in changedItems) {
				if (!current.IsEmpty() && !current.IsBroken()) {
					current.ForEachAttributeModifier(slot, (attribute, modifier) => {
						if (this.attributes.TryGetInstance(attribute, out AttributeInstance instance)) {
							instance.RemoveModifier(modifier.id);
							instance.AddTransientModifier(modifier);
						}
					});
				}
				this.lastEquipmentStacks[slot] = current;
			}
			this.OnEquipmentChanged(changedItems);
		}

		protected virtual void OnEquipmentChanged(Dictionary<EquipmentSlot, ItemStack> changedItems) {
		}

		private Dictionary<EquipmentSlot, ItemStack>? CollectEquipmentChanges() {
			Dictionary<EquipmentSlot, ItemStack>? changedItems = new();

			foreach (EquipmentSlot slot in EquipmentSlot.VALUES) {
				ItemStack previous = this.lastEquipmentStacks[slot];
				ItemStack current = this.GetStack(slot);
				if (this.HasEquipmentStackChanged(previous, current)) {
					changedItems ??= new Dictionary<EquipmentSlot, ItemStack>();
					changedItems.Add(slot, current);

					if (!previous.IsEmpty()) {
						previous.ForEachAttributeModifier(slot, (attribute, modifier) => {
							this.attributes.GetInstance(attribute)?.RemoveModifier(modifier);
						});
					}
				}
			}

			return changedItems;
		}

		public virtual bool HasEquipmentStackChanged(ItemStack previous, ItemStack current) {
			return !ItemStack.AreEqual(current, previous);
		}

		public void SetStack(EquipmentSlot slot, ItemStack stack) {
			this.OnEquipStack(slot, this.equipment.Set(slot, stack), stack);
		}

		public virtual void OnEquipStack(EquipmentSlot slot, ItemStack oldStack, ItemStack stack) {
		}

		public bool HasItemInSlot(EquipmentSlot slot) => !this.GetStack(slot).IsEmpty();

		public ItemStack GetStack(EquipmentSlot slot) => this.equipment.Get(slot);

		public virtual void OnEquippedItemBroke(Item brokenItem, EquipmentSlot slot) {
		}

		public virtual bool CanUse(EquipmentSlot slot) => true;

		public JToken Save() {
			JToken json = (JObject)SerializedData.CODEC.Encode(SerializedData.Get(this));
			json["type"] = EntityDescriptor.CODEC.Encode(this.descriptor);
			json["guid"] = Codecs.GUID.Encode(this.guid);
			this.SaveAdditional(json);
			return json;
		}

		protected virtual void SaveAdditional(JToken json) {
			json["attributes"] = AttributeInstance.Packed.CODEC.ListOf().Encode(this.attributes.Pack());
		}

		public void Load(JObject json) {
			SerializedData data = SerializedData.CODEC.Decode(json)
				.ResultOrPartial(error => Logger.LogError("Failed to load entity data: {}", error))
				.OrElse(SerializedData.Get(this));
			this.SetPosRaw(data.x, data.y);
			this.ReapplyPosition();
			this.SetDeltaMovement(data.motionX, data.motionY);
			this.SetOnGround(data.onGround);

			this.guid = Codecs.GUID.Decode(json["guid"] ?? new JValue(this.guid))
				.ResultOrPartial(error => Logger.LogError("Failed to load entity guid: {}", error))
				.OrElse(this.guid);
			this.LoadAdditional(json);
		}

		protected virtual void LoadAdditional(JObject json) {
			this.LoadAttributes(json);
		}

		private void LoadAttributes(JObject json) {
			JToken token = json["attributes"] ?? new JArray();
			this.attributes.Unpack(AttributeInstance.Packed.CODEC.ListOf().Decode(token)
				.ResultOrPartial(error => Logger.LogError("Failed to load entity attributes: {}", error))
				.OrElse(new List<AttributeInstance.Packed>())
			);
		}

		public static Entity? Load(JToken json, Level level) {
			if (json is not JObject obj) {
				Logger.LogError("Entity json is not object: {}", json);
				return null;
			}

			JToken typeToken = obj["type"] ?? JValue.CreateNull();
			Optional<EntityDescriptor> descriptor = EntityDescriptor.CODEC.Decode(typeToken)
				.ResultOrPartial(error => Logger.LogError("Invalid entity type: {} ({})", typeToken, error));
			if (descriptor.IsEmpty()) return null;

			Entity? entity = descriptor.GetValue().Create(level);
			if (entity == null) {
				Logger.LogError("Cannot create entity: {}. Parsed data: {}", typeToken, json);
				return null;
			}

			entity.Load(obj);
			return entity;
		}

		public sealed record SerializedData(double x, double y, double motionX, double motionY, bool onGround) {
			public static readonly Codec<SerializedData> CODEC = RecordCodec<SerializedData, double, double, double, double, bool>.Of(
				Field.Optional<SerializedData, double>("x", Codecs.DOUBLE, d => d.x, 0.0d),
				Field.Optional<SerializedData, double>("y", Codecs.DOUBLE, d => d.y, 0.0d),
				Field.Optional<SerializedData, double>("motionX", Codecs.DOUBLE, d => d.motionX, 0.0d),
				Field.Optional<SerializedData, double>("motionY", Codecs.DOUBLE, d => d.motionY, 0.0d),
				Field.Optional<SerializedData, bool>("onGround", Codecs.BOOLEAN, d => d.onGround, false),
				(x, y, motionX, motionY, onGround) => new SerializedData(x, y, motionX, motionY, onGround)
			);

			public static SerializedData Get(Entity entity) {
				return new SerializedData(
					entity.GetX(), entity.GetY(), entity.GetDeltaMovement().x, entity.GetDeltaMovement().y, entity.IsOnGround()
				);
			}
		}
	}
}
