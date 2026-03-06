using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.World.EntitySystem {
	public interface IEntityTransform {
		void Bind(Entity entity);
		Vector2 GetPos();
		void SetPos(Vector2 position);
	}
}
