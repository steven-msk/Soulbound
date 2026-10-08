namespace SoulboundEngine.World.Entity {
	using System;
	using System.Collections.Generic;

	public interface IEntityManager {
		void AddNewEntity(Entity entity);
		[Obsolete]
		void RemoveEntity(Entity entity);
		bool TryGetEntity(Guid guid, out Entity entity);
		IEnumerable<Entity> GetAllEntities();
	}
}
