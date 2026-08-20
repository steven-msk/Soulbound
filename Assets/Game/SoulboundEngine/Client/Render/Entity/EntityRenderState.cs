using SoulboundEngine.World.Entity;

namespace SoulboundEngine.Client.Render.Entity {
	using Entity = SoulboundEngine.World.Entity.Entity;

	public class EntityRenderState<E> where E : Entity {
		public EntityDescriptor<E> descriptor;
		public E entity;
	}
}
