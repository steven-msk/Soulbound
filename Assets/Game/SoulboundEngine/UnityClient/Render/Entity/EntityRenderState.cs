using SoulboundEngine.World.Entity;

namespace SoulboundEngine.UnityClient.Render.Entity {
	using Entity = SoulboundEngine.World.Entity.Entity;

	public class EntityRenderState<E> where E : Entity {
		public EntityDescriptor<E> descriptor;
		public E entity;
	}
}
