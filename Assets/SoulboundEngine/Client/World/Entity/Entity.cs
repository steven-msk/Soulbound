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

		public void AttachId(Guid guid) {
			this.guid = guid;

			transform = CreateTransform();
			transform.Bind(this);
			transform.SetPos(pos);
		}

		protected abstract IEntityTransform CreateTransform();

		public virtual Vector2 GetPos() => pos;
		public virtual void SetPos(Vector2 pos) {
			this.pos = pos;
			transform?.SetPos(pos);
		}

		public void Dispose() {
			OnDisposed();
			transform?.Destroy();
		}

		public bool IsAlive() => transform != null;

		protected virtual void OnDisposed() {
		}

		public EntityDescriptor GetEntityType() => descriptor;
	}
}
