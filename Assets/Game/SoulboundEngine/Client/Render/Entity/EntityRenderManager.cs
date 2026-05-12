using SoulboundEngine.Client.Render.Item;
using SoulboundEngine.Client.World.EntitySystem;
using System;
using System.Collections.Generic;

namespace SoulboundEngine.Client.Render.Entity {
	using Entity = World.EntitySystem.Entity;

	public sealed class EntityRenderManager {
		const string SCRIPTED_ENTITY_MODEL_LABEL = "entity_model";
		private readonly Func<EntityDescriptor, IEntityModelFactory> modelFactorySupplier;
		private readonly Dictionary<EntityDescriptor, EntityRenderer> renderers;
		private readonly Dictionary<Entity, RenderedEntity> renderedEntities = new();
		private readonly ScriptedEntityModelManager scriptedEntityModelManager;

		public EntityRenderManager(List<EntityDescriptor> descriptors, ItemRenderManager itemRenderManager) {
			this.scriptedEntityModelManager = new ScriptedEntityModelManager(SCRIPTED_ENTITY_MODEL_LABEL);
			this.scriptedEntityModelManager.LoadAll();

			EntityRenderer.FactoryContext context = new(this, itemRenderManager);
			this.renderers = EntityRenderers.GetRenderers(descriptors, context);
			this.modelFactorySupplier = EntityRenderers.GetModelFactorySupplier();
		}

		public IEntityView Render(Entity entity) {
			if (this.renderedEntities.ContainsKey(entity)) {
				this.Destroy(entity);
			}

			EntityRenderer renderer = this.GetRenderer(entity);
			IEntityModelFactory modelFactory = this.modelFactorySupplier(entity.GetDescriptor());
			IEntityModelFactory.Context modelFactoryContext = new(this.scriptedEntityModelManager);
			
			EntityModel model = modelFactory.GetModel(modelFactoryContext);
			object state = renderer.CreateRenderStateBoxed(entity);

			IEntityView view = renderer.CreateViewBoxed(state, model);
			if (!view.IsValid()) return null;

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
