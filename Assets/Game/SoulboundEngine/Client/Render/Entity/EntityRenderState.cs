using SoulboundEngine.Client.World.EntitySystem;

namespace SoulboundEngine.Client.Render.Entity {
	using Entity = World.EntitySystem.Entity;

	public class EntityRenderState<E> where E : Entity {
		public EntityDescriptor<E> descriptor;
		public E entity;
	}
}
