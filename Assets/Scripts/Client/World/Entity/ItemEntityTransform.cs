using SoulboundBackend.Client.ItemSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.World.EntitySystem {
	[RequireComponent(typeof(Rigidbody2D))]
	public class ItemEntityTransform : MonoBehaviour, IEntityTransform {
		private ItemEntity entity;
		private Rigidbody2D rb;

		public void Bind(Entity entity) {
			this.entity = (ItemEntity)entity;
			rb = GetComponent<Rigidbody2D>();
		}

		public void Destroy() {
			Destroy(gameObject);
		}

		Entity IEntityTransform.GetEntity() => entity;
		public ItemEntity GetEntity() => entity;

		public Vector2 GetPos() => transform.position;

		public void SetPos(Vector2 position) => transform.position = position;
	}
}
