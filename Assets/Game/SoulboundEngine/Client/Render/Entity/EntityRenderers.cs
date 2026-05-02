using SoulboundEngine.Client.World.EntitySystem;
using SoulboundEngine.Core.Assets;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulboundEngine.Client.Render.Entity {
	using Entity = World.EntitySystem.Entity;
	using Logger = Debug.Logging.Logger;

	public static class EntityRenderers {
		private static readonly Dictionary<EntityDescriptor, EntityModel.Factory> MODEL_FACTORIES = new();
		private static readonly Dictionary<EntityDescriptor, EntityRenderer.Factory> RENDERER_FACTORIES = new();

		static EntityRenderers() {
			Register(EntityType.PLAYER, () => new PlayerModel(AssetManager.Resolve<GameObject>(new AssetKey("player"))), context => new PlayerEntityRenderer(context));
			Register(EntityType.ITEM, () => new ItemEntityModel(), context => new ItemEntityRenderer(context));
		}

		public static void Register(EntityDescriptor descriptor, EntityModel.Factory modelFactory, EntityRenderer.Factory rendererFactory) {
			MODEL_FACTORIES.Add(descriptor, modelFactory);
			RENDERER_FACTORIES.Add(descriptor, rendererFactory);
		}

		public static Func<EntityDescriptor, EntityModel> GetModelSupplier() {
			return descriptor => MODEL_FACTORIES.GetValueOrDefault(descriptor, () => new MissingEntityModel())();
		}

		public static Dictionary<EntityDescriptor, EntityRenderer> GetRenderers(List<EntityDescriptor> descriptors, EntityRenderer.FactoryContext context) {
			Dictionary<EntityDescriptor, EntityRenderer> renderers = new();
			foreach (var descriptor in descriptors) {
				EntityRenderer.Factory factory = RENDERER_FACTORIES.GetValueOrDefault(
					descriptor,
					context => {
						Logger.LogError("Renderer not found: {}", descriptor);
						return new EmptyEntityRenderer<Entity>(context);
					}
				);
				renderers[descriptor] = factory(context);
			}
			return renderers;
		}
	}
}
