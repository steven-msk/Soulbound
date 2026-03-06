using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.World.EntitySystem {
	public abstract class Entity : IDisposable {
		public Guid guid { get; private set; }
		protected Level level;
		protected IEntityTransform transform;
		protected Vector2 pos;
		protected readonly Vector2 initialPos;

		protected Entity(Vector2 initialPos) {
			this.pos = this.initialPos = initialPos;
		}

		public void AttachToLevel(Level level, Guid guid) {
			this.guid = guid;
			this.level = level;

			transform = CreateTransform();
			transform.Bind(this);
			transform.SetPos(pos);
		}

		protected abstract IEntityTransform CreateTransform();

		public Vector2 GetPos() => pos;
		public void SetPos(Vector2 pos) {
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
