using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.World.EntitySystem {
	[PROTOTYPICAL]
	internal class StaticTransform : MonoBehaviour, IEntityTransform {
		public void Bind(Entity entity) {
		}

		public void Destroy() => Destroy(gameObject);

		public Vector2 GetPos() => transform.position;

		public void SetPos(Vector2 position) => transform.position = position;
	}
}
