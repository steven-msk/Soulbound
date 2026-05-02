using SoulboundEngine.Client.World.EntitySystem;
using UnityEngine;

namespace SoulboundEngine.Client.Render.Entity {
	using Entity = World.EntitySystem.Entity;

	public sealed class EmptyEntityRenderer<E> : EntityRenderer<E, EntityRenderState<E>, EntityModel> where E : Entity {
		public EmptyEntityRenderer(FactoryContext context) 
			: base(context) {
		}

		public override EntityRenderState<E> CreateRenderState(E entity) {
			return new EntityRenderState<E> {
				descriptor = (EntityDescriptor<E>)entity.GetDescriptor(),
				entity = entity
			};
		}

		public override IEntityView CreateView(EntityRenderState<E> state, EntityModel model) {
			GameObject obj = new("Entity");
			obj.transform.position = state.entity.GetPosition();
			return IEntityView.Of(obj);
		}

		public override void DestroyView(IEntityView view) {
			view.Destroy();
		}

		public override void UpdateView(EntityRenderState<E> state, IEntityView view) {
		}
	}
}
