namespace SoulboundEngine.World.Entity {
	using Newtonsoft.Json.Linq;
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.Common.Math.Random;
	using SoulboundEngine.Item;
	using SoulboundEngine.Registry;
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

		protected void AssertAlive() {
			if (!this.isAlive) throw new NotSupportedException("Entity is not alive.");
		}

		protected virtual void OnDisposed() {
		}

		public EntityDescriptor GetDescriptor() => this.descriptor;

		public ItemEntity DropItem(Level level, IItemConvertible item) {
			return this.DropStack(level, item.AsItem().GetDefaultStack(1));
		}

		public ItemEntity DropStack(Level level, ItemStack stack) {
			Vec2d pos = this.GetPosition();
			ItemEntity entity = new(level, pos.x, pos.y, stack);
			entity.SetOwner(this);
			level.AddNewEntity(entity);
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

		public JToken Save() {
			JObject json = new() {
				["type"] = EntityDescriptor.GetIdentifier(this.descriptor).ToString(),
				["descriptionId"] = this.guid.ToString(),
				["x"] = this.GetX(),
				["y"] = this.GetY(),
				["motionX"] = this.GetDeltaMovement().x,
				["motionY"] = this.GetDeltaMovement().y,
				["onGround"] = this.isOnGround,
			};
			this.SaveAdditional(json);
			return json;
		}

		protected virtual void SaveAdditional(JObject json) {
		}

		public void Load(JObject json) {
			double? x = (double?)json["x"];
			if (x == null) {
				Logger.LogError("No x property found on Entity json: {}", json);
				return;
			}
			double? y = (double?)json["y"];
			if (y == null) {
				Logger.LogError("No y property found on Entity json: {}", json);
				return;
			}

			double? motionX = (double?)json["motionX"];
			if (motionX == null) {
				Logger.LogError("No motionX property found on Entity json: {}", json);
				return;
			}
			double? motionY = (double?)json["motionY"];
			if (motionY == null) {
				Logger.LogError("No motionY property found on Entity json: {}", json);
				return;
			}

			bool? onGround = (bool?)json["onGround"];
			if (onGround == null) {
				Logger.LogError("No onGround property found on Entity json: {}", json);
				return;
			}


			this.SetPosRaw(x.GetValueOrDefault(0.0d), y.GetValueOrDefault(0.0d));
			this.ReapplyPosition();
			this.SetDeltaMovement(motionX.GetValueOrDefault(0.0d), motionY.GetValueOrDefault(0.0d));
			this.SetOnGround(onGround.GetValueOrDefault(false));

			string? guidString = (string?)json["descriptionId"];
			if (guidString != null) {
				if (!Guid.TryParse(guidString, out Guid guid)) {
					Logger.LogError("Failed to parse entity guid: {}", guidString);
				} else {
					this.guid = guid;
				}
			}

			this.LoadAdditional(json);
		}

		protected virtual void LoadAdditional(JObject json) {
		}

		public static Entity? Load(JToken json, Level level) {
			if (json.Type != JTokenType.Object) {
				Logger.LogError("Entity json is not object: {}", json);
				return null;
			}

			string? typeIdString = (string?)json["type"];
			if (typeIdString == null) {
				Logger.LogError("No type property found on Entity json: {}", json);
				return null;
			}
			if (!Identifier.TryParse(typeIdString, out Identifier typeId)) {
				Logger.LogError("Could not parse Entity type descriptionId: {}", typeIdString);
				return null;
			}
			EntityDescriptor? descriptor = EntityDescriptor.Get(typeId);
			if (descriptor == null) {
				Logger.LogError("Entity descriptor not found: {}", typeIdString);
				return null;
			}

			Entity? entity = descriptor.Create(level);
			if (entity == null) {
				Logger.LogError("Cannot create entity: {}. Parsed data: {}", typeIdString, json);
				return null;
			}

			entity.Load((JObject)json);
			return entity;
		}
	}
}
