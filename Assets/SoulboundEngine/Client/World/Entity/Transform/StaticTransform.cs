using SoulboundEngine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundEngine.Client.World.EntitySystem.Transform {
	[PROTOTYPICAL]
	internal class StaticTransform : MonoBehaviour, IEntityTransform {
		private Entity entity;

		public void Bind(Entity entity) => this.entity = entity;

		public void Destroy() => Destroy(gameObject);

		public Entity GetEntity() => entity;

		public Vector2 GetPos() => transform.position;

		public void SetPos(Vector2 position) => transform.position = position;
	}
}
