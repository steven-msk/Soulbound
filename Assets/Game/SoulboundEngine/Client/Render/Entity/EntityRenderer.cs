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
			S s = (S)state;
			EntityViewHandle handle = this.Create(s, (M)model);
			this.UpdatePosition(s, in handle);
			return handle;
		}

		internal sealed override void Update(object state, in EntityViewHandle handle) {
			S s = (S)state;
			this.UpdatePosition(s, in handle);
			this.Update(s, in handle);
		}

		protected virtual void UpdatePosition(S state, in EntityViewHandle handle) {
			handle.SetPosition(state.entity.GetPosition());
		}

		public override void Destroy(in EntityViewHandle handle) {
			GameObject.Destroy(handle.GetGameObject());
		}
	}
}
