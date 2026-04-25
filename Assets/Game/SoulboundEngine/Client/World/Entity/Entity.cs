using SoulboundEngine.Client.World.EntitySystem.Transform;
using SoulboundEngine.Client.World.LevelDomain;
using System;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem {
	public abstract class Entity : IDisposable {
		public Guid guid { get; private set; }
		private readonly EntityDescriptor descriptor;
		protected Level level;
		protected IEntityTransform? transform;
		private Vector2 pos;

		protected Entity(EntityDescriptor descriptor, Level level) {
			this.descriptor = descriptor;
			this.level = level;
		}

		public void OnAdd(Guid guid) {
			if (this.IsAlive()) throw new InvalidOperationException($"Entity already added: {guid}");

			this.guid = guid;
			this.transform = this.descriptor.CreateTransform(this);
			this.transform.SetPos(this.pos);
			this.OnTransformCreated(this.transform);
		}

		protected virtual void OnTransformCreated(IEntityTransform transform) {
		}

		public virtual void FrameUpdate() {
			this.transform?.FrameUpdate();
		}

		public virtual Vector2 GetPos() => this.pos;
		public virtual void SetPos(Vector2 pos) {
			this.pos = pos;
			this.transform?.SetPos(pos);
		}

		public void Dispose() {
			this.OnDisposed();
			this.transform?.Destroy();
		}

		public bool IsAlive() => this.transform != null;

		protected virtual void OnDisposed() {
		}

		public EntityDescriptor GetDescriptor() => this.descriptor;
	}
}
