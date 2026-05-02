namespace SoulboundEngine.Client.Render.Entity {
	using SoulboundEngine.Client.Render.Item;
	using Entity = World.EntitySystem.Entity;

	public abstract class EntityRenderer {
		protected readonly EntityRenderManager entityRenderManager;

		protected EntityRenderer(FactoryContext context) {
			this.entityRenderManager = context.entityRenderManager;
		}

		internal abstract object CreateRenderStateBoxed(Entity entity);

		internal abstract IEntityView CreateViewBoxed(object state, EntityModel model);
		internal abstract void UpdateViewBoxed(object state, IEntityView view);
		public abstract void DestroyView(IEntityView view);

		public sealed record FactoryContext(EntityRenderManager entityRenderManager, ItemRenderManager itemRenderManager);

		public delegate EntityRenderer<E, S, M> Factory<E, S, M>(FactoryContext context) where E : Entity where S : EntityRenderState<E> where M : EntityModel;

		public interface IFactory { 
			EntityRenderer GetRenderer(FactoryContext context);

			public static IFactory Of<E, S, M>(Factory<E, S, M> factory) where E : Entity where S : EntityRenderState<E> where M : EntityModel {
				return new DelegateImpl<E, S, M>(factory);
			}

			private sealed class DelegateImpl<E, S, M> : IFactory where E : Entity where S : EntityRenderState<E> where M : EntityModel {
				private readonly Factory<E, S, M> factory;

				public DelegateImpl(Factory<E, S, M> factory) {
					this.factory = factory;
				}

				public EntityRenderer GetRenderer(FactoryContext context) => this.factory(context);
			}
		}
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
