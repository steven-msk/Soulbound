using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.World.EntitySystem {
	[RequireComponent(typeof(Rigidbody2D))]
	public class PhysicsTransform : MonoBehaviour, IEntityTransform {
		private Entity entity;
		private Rigidbody2D body;

		public void Bind(Entity entity) {
			this.entity = entity;
			body = GetComponent<Rigidbody2D>();
		}

		public void Destroy() => Destroy(gameObject);

		public Vector2 GetPos() => body.position;

		public void SetPos(Vector2 position) {
			body.position = position;
		}

		private void FixedUpdate() {
			entity.SetPos(body.position);
		}
	}
}
