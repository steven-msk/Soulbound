using SoulboundEngine.Client.World.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundEngine.Client.Runtime.Services {
	public interface IEntityExecutionService {
		void SetPos(Guid entityGuid, Vector2 pos);
		void AddEntity(Entity entity);
	}
}
