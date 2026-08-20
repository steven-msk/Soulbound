using SoulboundEngine.Common.Math;
using System;

namespace SoulboundEngine.World.Services {
	using Entity = Entity.Entity;

	public interface IEntityExecutionService {
		void SetPos(Guid entityGuid, Vec2d pos);
		void AddEntity(Entity entity);
	}
}
