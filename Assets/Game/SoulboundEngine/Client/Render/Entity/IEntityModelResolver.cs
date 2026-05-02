namespace SoulboundEngine.Client.Render.Entity {
	using Entity = World.EntitySystem.Entity;

	public interface IEntityModelResolver {
		EntityModel Resolve(Entity entity);

		public delegate IEntityModelResolver Factory();

		public sealed class Missing : IEntityModelResolver {

			public EntityModel Resolve(Entity entity) {
				return new MissingEntityModel();
			}
		}
	}
}
