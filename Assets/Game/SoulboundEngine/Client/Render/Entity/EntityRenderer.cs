namespace SoulboundEngine.Client.Render.Entity {
	using Entity = World.EntitySystem.Entity;

	public abstract class EntityRenderer {
		public delegate EntityRenderer Factory();

		internal abstract object CreateRenderStateBoxed(Entity entity);

		internal abstract IEntityView CreateViewBoxed(object state, EntityModel model);
		internal abstract void UpdateViewBoxed(object state, IEntityView view);
		public abstract void DestroyView(IEntityView view);
	}

	public abstract class EntityRenderer<E, S, M> : EntityRenderer where E : Entity where S : EntityRenderState<E> where M : EntityModel {
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
