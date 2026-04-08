using SoulboundEngine.Common;
using System;
using UnityEngine;

namespace SoulboundEngine.Client.World.EntitySystem.Transform {
	[PROTOTYPICAL]
	[Obsolete]
	internal class StaticTransform : MonoBehaviour, IEntityTransform {
		// currently the transform leaves the implementation hidden for physics transforms.
		// this encapsulation doesnt match the default way of entities to express their state.
		// so PlayerTransform, PhysicsTransform and StaticTransform are obsolete because of this.

		private Entity entity;

		public void Bind(Entity entity) => this.entity = entity;

		public void Destroy() => Destroy(gameObject);

		public Entity GetEntity() => entity;

		public Vector2 GetPos() => transform.position;

		public void SetPos(Vector2 position) => transform.position = position;

		void IEntityTransform.FrameUpdate() {
		}
	}
}
