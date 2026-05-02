namespace SoulboundEngine.Client.Render.Entity {
	using SoulboundEngine.Client.Render.Item;
	using Entity = World.EntitySystem.Entity;

	public abstract class EntityRenderer {
		public delegate EntityRenderer Factory(FactoryContext context);

		protected readonly EntityRenderManager entityRenderManager;

		protected EntityRenderer(FactoryContext context) {
			this.entityRenderManager = context.entityRenderManager;
		}

		internal abstract object CreateRenderStateBoxed(Entity entity);

		internal abstract IEntityView CreateViewBoxed(object state, EntityModel model);
		internal abstract void UpdateViewBoxed(object state, IEntityView view);
		public abstract void DestroyView(IEntityView view);

		public sealed record FactoryContext(EntityRenderManager entityRenderManager, ItemRenderManager itemRenderManager);
	}

	public abstract class EntityRenderer<E, S, M> : EntityRenderer where E : Entity where S : EntityRenderState<E> where M : EntityModel {
		protected EntityRenderer(FactoryContext context) 
			: base(context) {
		}

		public abstract S CreateRenderState(E entity);

		public abstract IEntityView CreateView(S state, M model);
		public abstract void UpdateView(S state, IEntityView view);

		internal override object CreateRenderStateBoxed(Entity entity) {
			return this.CreateRenderState((E)entity);
		}
		internal override IEntityView CreateViewBoxed(object state, EntityModel model) {
			return this.CreateView((S)state, (M)model);
		}
		internal override void UpdateViewBoxed(object state, IEntityView view) {
			this.UpdateView((S)state, view);
		}
	}
}
