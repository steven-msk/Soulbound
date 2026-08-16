namespace SoulboundEngine.Client.Render.Entity {
	using SoulboundEngine.Client.Render.Item;
	using System;
	using UnityEngine;
	using Entity = SoulboundEngine.World.Entity.Entity;

	public abstract class EntityRenderer {
		protected readonly EntityRenderManager entityRenderManager;

		protected EntityRenderer(FactoryContext context) {
			this.entityRenderManager = context.entityRenderManager;
		}

		internal abstract object CreateRenderState(Entity entity);

		internal abstract EntityViewHandle Create(object state, EntityModel model);

		internal abstract void Update(object state, in EntityViewHandle handle);

		public abstract void Destroy(in EntityViewHandle handle);

		public sealed record FactoryContext(
			EntityRenderManager entityRenderManager, 
			ItemRenderManager itemRenderManager
		);

		public record Factory(Func<FactoryContext, EntityRenderer> function) {
			public static Factory Of<E, S, M>(EntityRenderer<E, S, M>.Factory rendererFactory)
					where E : Entity where S : EntityRenderState<E> where M : EntityModel {
				return new Factory(context => rendererFactory(context));
			}

			public EntityRenderer Apply(FactoryContext context) => this.function(context);
		}
	}

	public abstract class EntityRenderer<E, S, M> : EntityRenderer where E : Entity where S : EntityRenderState<E> where M : EntityModel {
		public new delegate EntityRenderer<E, S, M> Factory(FactoryContext context);

		protected EntityRenderer(FactoryContext context) 
			: base(context) {
		}

		public abstract S CreateRenderState(E entity);

		public abstract EntityViewHandle Create(S state, M model);

		public virtual void Update(S state, in EntityViewHandle handle) {
		}

		internal sealed override object CreateRenderState(Entity entity) {
			return this.CreateRenderState((E)entity);
		}

		internal sealed override EntityViewHandle Create(object state, EntityModel model) {
			return this.Create((S)state, (M)model);
		}

		internal sealed override void Update(object state, in EntityViewHandle handle) {
			this.Update((S)state, in handle);
		}

		public override void Destroy(in EntityViewHandle handle) {
			GameObject.Destroy(handle.GetGameObject());
		}
	}
}
