using System;
using UnityEngine;

namespace SoulboundEngine.World.Services {
	using Entity = Entity.Entity;

	public interface IEntityExecutionService {
		void SetPos(Guid entityGuid, Vector2 pos);
		void AddEntity(Entity entity);
	}
}
