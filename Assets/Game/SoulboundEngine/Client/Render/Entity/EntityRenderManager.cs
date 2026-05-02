using SoulboundEngine.Client.World.EntitySystem;
using System;
using System.Collections.Generic;

namespace SoulboundEngine.Client.Render.Entity {
	using Entity = World.EntitySystem.Entity;

	public sealed class EntityRenderManager {
		private readonly Func<EntityDescriptor, IEntityModelResolver> modelResolverFactory;
		private readonly Dictionary<EntityDescriptor, EntityRenderer> renderers;
		private readonly Dictionary<Entity, RenderedEntity> renderedEntities = new();

		public EntityRenderManager(List<EntityDescriptor> descriptors) {
			this.renderers = EntityRenderers.GetRenderers(descriptors);
			this.modelResolverFactory = EntityRenderers.GetModelResolverFactory();
		}

		public IEntityView Render(Entity entity) {
			if (this.renderedEntities.ContainsKey(entity)) {
				this.Destroy(entity);	
			}

			IEntityModelResolver modelResolver = this.modelResolverFactory(entity.GetDescriptor());
			EntityRenderer renderer = this.GetRenderer(entity);

			EntityModel model = modelResolver.Resolve(entity);
			object state = renderer.CreateRenderStateBoxed(entity);
			IEntityView view = renderer.CreateViewBoxed(state);

			this.renderedEntities[entity] = new RenderedEntity(entity, state, view);
			return view;
		}

		public void Update(Entity entity) {
			if (!this.renderedEntities.TryGetValue(entity, out RenderedEntity renderedEntity)) return;

			this.GetRenderer(entity).UpdateViewBoxed(renderedEntity.state, renderedEntity.view);
		}

		public void Destroy(Entity entity) {
			if (this.renderedEntities.Remove(entity, out RenderedEntity renderedEntity)) {
				this.GetRenderer(entity).DestroyView(renderedEntity.view);
			}
		}
		public EntityRenderer GetRenderer(Entity entity) {
			return this.renderers[entity.GetDescriptor()];
		}

		internal sealed record RenderedEntity(Entity entity, object state, IEntityView view);
	}
}
