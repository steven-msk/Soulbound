using SoulboundEngine.Client.World.EntitySystem.Transform;
using SoulboundEngine.Client.World.LevelDomain;
using System;
using UnityEngine;

namespace SoulboundEngine.Client.World.EntitySystem {
	public abstract class Entity : IDisposable {
		public Guid guid { get; private set; }
		public readonly EntityDescriptor descriptor;
		protected Level level;
		protected IEntityTransform transform;
		private Vector2 pos;
		protected readonly Vector2 initialPos;

		protected Entity(EntityDescriptor descriptor, Vector2 initialPos) {
			this.pos = this.initialPos = initialPos;
			this.descriptor = descriptor;
		}

		public void AttachToLevel(Level level, Guid guid) {
			this.guid = guid;
			this.level = level;

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

		protected virtual void OnDisposed() {
		}
	}
}
