using SoulboundEngine.Common.Math;
using SoulboundEngine.Item;
using SoulboundEngine.World.Block;
using SoulboundEngine.World.Block.State;
using SoulboundEngine.World.Chunk;
using SoulboundEngine.World.Physics;
using SoulboundEngine.World.Player;
using System;
using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.World.Entity {
	using Level = Level.Level;

	public abstract class Entity {
		public const double DEFAULT_BB_WIDTH = 1.0d;
		public const double DEFAULT_BB_HEIGHT = 2.0d;
		public const float DEFAULT_BLOCK_FRICTION = 0.6f;
		public const float CONSTANT_DECELERATION = 0.91f;
		public static readonly Predicate<Entity> CAN_BE_COLLIDED_WITH = e => e.CanBeCollidedWith(null);
		public static readonly Predicate<Entity> ALL = _ => true;
		private readonly EntityDescriptor descriptor;
		private readonly EntityDimensions dimensions;
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
		private float speed;

		protected Entity(EntityDescriptor descriptor, Level level) {
			this.descriptor = descriptor;
			this.level = level;
			this.dimensions = descriptor.GetDimensions();
			this.SetPos(0.0d, 0.0d);
		}

		public Vec2d position { get; private set; } = Vec2d.ZERO;
		public AABB boundingBox { get; private set; }
		public Facing facing { get; private set; } = Facing.RIGHT;
		public Guid guid { get; private set; }
		public BlockPos blockPosition { get; private set; }
		public ChunkPos chunkPosition { get; private set; }

		public void OnAdd(Guid guid) {
			if (this.IsAlive()) throw new InvalidOperationException($"Entity already added: {guid}");

			this.guid = guid;
			this.isAlive = true;
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
			this.isAlive = false;
		}

		public bool IsAlive() => this.isAlive;

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
			ItemEntity entity = new(this, stack, level);
			entity.SetPos(this.GetPosition());
			level.AddEntity(entity);
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

			Vec2d dm = this.deltaMovement;
			this.deltaMovement = new Vec2d(xCollision ? 0.0d : dm.x, yCollision ? 0.0d : dm.y);
		}

		private Vec2d Collide(Vec2d movement) {
			if (movement.lengthSqr < 1.0E-7) return movement;

			AABB box = this.boundingBox;
			List<AABB> colliders = CollectColliders(this, this.level, box.ExpandBy(movement));
			if (colliders.Count == 0) return movement;

			return CollideWithShapes(movement, box, colliders);
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

		public virtual float GetSpeed() => this.speed;
		public virtual void SetSpeed(float speed) {
			this.speed = speed;
		}

		protected virtual double GetGravity() {
			return 0.0d;
		}

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

		public Vec2d GetPosition() => this.position;
		public double GetX() => this.position.x;
		public double GetY() => this.position.y;

		public void SetFacing(Facing facing) {
			this.facing = facing;
		}
	}
}
