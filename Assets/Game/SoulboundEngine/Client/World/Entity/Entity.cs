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
		protected readonly TransformAdapter transformAdapter;

		protected Entity(EntityDescriptor descriptor, Level level) {
			this.transformAdapter = new TransformAdapter(this);
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

		public virtual Vector2 GetPosition() => this.pos;
		public virtual void SetPosition(Vector2 pos) {
			this.pos = pos;
			this.transformAdapter?.SetPosition(pos);
		}

		public ItemEntity DropItem(Level level, IItemConvertible item) {
			return this.DropStack(level, item.AsItem().CreateStack(1));
		}

		public ItemEntity DropStack(Level level, ItemStack stack) {
			ItemEntity entity = new(this, stack, level);
			entity.SetPosition(this.GetPosition());
			return entity;
		}

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

		public void SyncPhysicalPosition() => this.transformAdapter?.SyncPhysicsPosition();

		public void SetPhysicsHandle(IPhysicsHandle? physicsHandle) {
			this.transformAdapter.SetPhysicsHandle(physicsHandle);
		}
		public void SetBoundingBoxHandle(IBoundingBoxHandle? boundingBoxHandle) {
			this.transformAdapter.SetBoundingBoxHandle(boundingBoxHandle);
		}

		public void SetVelocity(Vector2 velocity) {
			this.AssertAlive();
			this.transformAdapter.SetVelocity(velocity);
		}
		public Vector2 GetVelocity() {
			this.AssertAlive();
			return this.transformAdapter.GetVelocity();
		}
		public float GetVelocityX() => this.GetVelocity().x;
		public float GetVelocityY() => this.GetVelocity().y;
		public void SetVelocityX(float velocityX) {
			Vector2 velocity = this.GetVelocity();
			this.SetVelocity(new Vector2(velocityX, velocity.y));
		}
		public void SetVelocityY(float velocityY) {
			Vector2 velocity = this.GetVelocity();
			this.SetVelocity(new Vector2(velocity.x, velocityY));
		}

		public void SetNormalVelocity(Vector2 normalVelocity) {
			this.AssertAlive();
			this.transformAdapter.SetNormalVelocity(normalVelocity);
		}
		public Vector2 GetNormalVelocity() {
			this.AssertAlive();
			return this.transformAdapter.GetNormalVelocity();
		}
		public float GetNormalVelocityX() => this.GetNormalVelocity().x;
		public float GetNormalVelocityY() => this.GetNormalVelocity().y;
		public void SetNormalVelocityX(float normalVelocityX) {
			Vector2 normalVelocity = this.GetNormalVelocity();
			this.SetNormalVelocity(new Vector2(normalVelocityX, normalVelocity.y));
		}
		public void SetNormalVelocityY(float normalVelocityY) {
			Vector2 normalVelocity = this.GetNormalVelocity();
			this.SetNormalVelocity(new Vector2(normalVelocity.x, normalVelocityY));
		}

		public void ApplyForce(Vector2 force) {
			this.AssertAlive();
			this.transformAdapter.ApplyForce(force);
		}
		public void ApplyForceX(float forceX) => this.ApplyForce(new Vector2(forceX, 0f));
		public void ApplyForceY(float forceY) => this.ApplyForce(new Vector2(0f, forceY));

		public Bounds GetBoundingBox() {
			this.AssertAlive();
			return this.transformAdapter.GetBoundingBox();
		}

		public Vector2 GetCenter() => this.GetBoundingBox().center;

		protected class TransformAdapter {
			private readonly Entity entity;
			private IPhysicsHandle? physicsHandle;
			private IBoundingBoxHandle? boundingBoxHandle;

			public TransformAdapter(Entity entity) {
				this.entity = entity;
			}

			public void SyncPhysicsPosition() {
				if (this.physicsHandle == null) return;
				this.entity.pos = this.physicsHandle.GetPosition();
			}

			public void SetPhysicsHandle(IPhysicsHandle? physicsHandle) {
				this.physicsHandle = physicsHandle;
				this.SetPosition(this.entity.pos);
			}

			public void SetBoundingBoxHandle(IBoundingBoxHandle? boundingBoxHandle) {
				this.boundingBoxHandle = boundingBoxHandle;
			}

			public void SetPosition(Vector2 pos) {
				this.physicsHandle?.SetPosition(pos);
			}

			public Bounds GetBoundingBox() => this.boundingBoxHandle?.GetBoundingBox() ?? default;

			public void SetNormalVelocity(Vector2 normalVelocity) => this.physicsHandle?.SetNormalVelocity(normalVelocity);
			public Vector2 GetNormalVelocity() => this.physicsHandle?.GetNormalVelocity() ?? Vector2.zero;
			public void SetVelocity(Vector2 velocity) => this.physicsHandle?.SetVelocity(velocity);
			public Vector2 GetVelocity() => this.physicsHandle?.GetVelocity() ?? Vector2.zero;

			public void ApplyForce(Vector2 force) => this.physicsHandle?.ApplyForce(force);
		}

		public interface IPhysicsHandle {
			void SetVelocity(Vector2 velocity);
			Vector2 GetVelocity();
			public float GetVelocityX() => this.GetVelocity().x;
			public float GetVelocityY() => this.GetVelocity().y;
			public void SetVelocityX(float velocityX) {
				Vector2 velocity = this.GetVelocity();
				this.SetVelocity(new Vector2(velocityX, velocity.y));
			}
			public void SetVelocityY(float velocityY) {
				Vector2 velocity = this.GetVelocity();
				this.SetVelocity(new Vector2(velocity.x, velocityY));
			}

			void SetNormalVelocity(Vector2 normalVelocity);
			Vector2 GetNormalVelocity();
			public float GetNormalVelocityX() => this.GetNormalVelocity().x;
			public float GetNormalVelocityY() => this.GetNormalVelocity().y;
			public void SetNormalVelocityX(float normalVelocityX) {
				Vector2 normalVelocity = this.GetNormalVelocity();
				this.SetNormalVelocity(new Vector2(normalVelocityX, normalVelocity.y));
			}
			public void SetNormalVelocityY(float normalVelocityY) {
				Vector2 normalVelocity = this.GetNormalVelocity();
				this.SetNormalVelocity(new Vector2(normalVelocity.x, normalVelocityY));
			}

			Vector2 GetPosition();
			void SetPosition(Vector2 pos);

			void ApplyForce(Vector2 force);
			public void ApplyForceX(float forceX) => this.ApplyForce(new Vector2(forceX, 0f));
			public void ApplyForceY(float forceY) => this.ApplyForce(new Vector2(0f, forceY));
		}

		public interface IBoundingBoxHandle {
			Bounds GetBoundingBox();
			public Vector2 GetCenter() => this.GetBoundingBox().center;
		}
	}
}
