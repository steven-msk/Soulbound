using SoulboundEngine.UnityClient.Debug.Logging;
using SoulboundEngine.UnityClient.Render.Item;
using SoulboundEngine.World.Entity;
using System;
using System.Collections.Generic;

namespace SoulboundEngine.UnityClient.Render.Entity {
	using Entity = SoulboundEngine.World.Entity.Entity;

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

		public void Render(Entity entity) {
			if (this.renderedEntities.ContainsKey(entity)) {
				this.Destroy(entity);
			}

			EntityRenderer renderer = this.GetRenderer(entity);
			IEntityModelFactory modelFactory = this.modelFactorySupplier(entity.GetDescriptor());
			IEntityModelFactory.Context modelFactoryContext = new(this.scriptedEntityModelManager);
			
			EntityModel model = modelFactory.GetModel(modelFactoryContext);
			object state = renderer.CreateRenderState(entity);

			EntityViewHandle handle = renderer.Create(state, model);
			if (!handle.IsValid()) {
				throw new InvalidOperationException("An invalid entity view handle was created");
			}

			this.renderedEntities[entity] = new RenderedEntity(state, handle);
		}

		public void Update(Entity entity) {
			if (!this.renderedEntities.TryGetValue(entity, out RenderedEntity renderedEntity)) {
				Logger.LogWarning("Cannot update entity {} because it has not been created. Please call Render(Entity) first", entity);
				return;
			}

			EntityViewHandle handle = renderedEntity.handle;
			this.GetRenderer(entity).Update(renderedEntity.state, handle);
		}

		public void Destroy(Entity entity) {
			if (this.renderedEntities.Remove(entity, out RenderedEntity renderedEntity)) {
				EntityViewHandle handle = renderedEntity.handle;
				this.GetRenderer(entity).Destroy(in handle);
			}
		}

		public EntityRenderer GetRenderer(Entity entity) {
			return this.renderers[entity.GetDescriptor()];
		}

		internal sealed record RenderedEntity(object state, in EntityViewHandle handle);
	}
}
