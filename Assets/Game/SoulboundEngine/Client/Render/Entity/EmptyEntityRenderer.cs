using SoulboundEngine.Common.Math;
using UnityEngine;

namespace SoulboundEngine.Client.Render.Entity {
	using Entity = SoulboundEngine.World.Entity.Entity;

	public sealed class EmptyEntityRenderer<E> : EntityRenderer<E, EntityRenderState<E>, EntityModel> where E : Entity {
		public EmptyEntityRenderer(FactoryContext context) 
			: base(context) {
		}

		public override EntityRenderState<E> CreateRenderState(E entity) {
			return new EntityRenderState<E> { entity = entity };
		}

		public override EntityViewHandle Create(EntityRenderState<E> state, EntityModel model) {
			GameObject obj = new("Entity");
			Vec2d pos = state.entity.GetPosition();
			obj.transform.position = new Vector3((float)pos.x, (float)pos.y);
			return EntityViewHandle.Of(obj);
		}
	}
}
