using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundEngine.Client.World.EntitySystem.Transform {
	public interface IEntityTransform {
		void Bind(Entity entity);
		Entity GetEntity();
		Vector2 GetPos();
		void SetPos(Vector2 position);
		void Destroy();
	}
}
