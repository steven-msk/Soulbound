using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.World.LevelDomain;
using System;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem {
	public abstract class Entity {
		public Guid guid { get; private set; }
		private readonly EntityDescriptor descriptor;
		protected Level level;
		private Vector2 pos;
		protected bool isAlive;
		protected readonly PositionSynchronizer positionSynchronizer;

		protected Entity(EntityDescriptor descriptor, Level level) {
			this.positionSynchronizer = new PositionSynchronizer(this);
			this.descriptor = descriptor;
			this.level = level;
		}

		public void OnAdd(Guid guid) {
			if (this.IsAlive()) throw new InvalidOperationException($"Entity already added: {guid}");

			this.guid = guid;
			this.isAlive = true;
		}

		public virtual void FrameUpdate() {
		}

		public virtual Vector2 GetPos() => this.pos;
		public virtual void SetPos(Vector2 pos) {
			this.pos = pos;
		}

		public ItemEntity DropItem(Level level, IItemConvertible item) {
			return this.DropStack(level, item.AsItem().CreateStack(1));
		}

		public ItemEntity DropStack(Level level, ItemStack stack) {
			ItemEntity entity = new(this, stack, level);
			entity.SetPos(this.GetPos());
			return entity;
		}

		public void Dispose() {
			this.OnDisposed();
			this.isAlive = false;
		}

		public bool IsAlive() => this.isAlive;

		protected virtual void OnDisposed() {
		}

		public EntityDescriptor GetDescriptor() => this.descriptor;

		public void SetPhysicsHandle(IPhysicsHandle? physicsHandle) {
			this.positionSynchronizer.SetPhysicsHandle(physicsHandle);
		}

		protected sealed class PositionSynchronizer {
			private readonly Entity entity;
			private IPhysicsHandle? physicsHandle;

			public PositionSynchronizer(Entity entity) {
				this.entity = entity;
			}

			public void SetPhysicsHandle(IPhysicsHandle? handle) {
				this.physicsHandle = handle;
			}

			public void SyncPhysicsPosition() {
				if (this.physicsHandle == null) return;
				this.entity.pos = this.physicsHandle.GetPosition();
			}
		}

		public interface IPhysicsHandle {
			void SetVelocity(Vector2 velocity);
			Vector2 GetVelocity();
			public float GetVelocityX() => this.GetVelocity().x;
			public float GetVelocityY() => this.GetVelocity().y;

			Vector2 GetPosition();

			void ApplyForce(Vector2 force);
			public void ApplyForceX(float forceX) => this.ApplyForce(new Vector2(forceX, 0f));
			public void ApplyForceY(float forceY) => this.ApplyForce(new Vector2(0f, forceY));
		}
	}
}
