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
		private static readonly Dictionary<EntityDescriptor, EntityRenderer.IFactory> RENDERER_FACTORIES = new();

		static EntityRenderers() {
			Register(EntityType.PLAYER, 
				() => new PlayerModel(AssetManager.Resolve<GameObject>(new AssetKey("player"))),
				context => new PlayerEntityRenderer(context)
			);
			Register(EntityType.ITEM,
				() => new ItemEntityModel(),
				context => new ItemEntityRenderer(context)
			);
		}

		public static void Register<E, S, M>(EntityDescriptor<E> descriptor, EntityModel.Factory modelFactory, EntityRenderer.Factory<E, S, M> rendererFactory)
				where E : Entity where S : EntityRenderState<E> where M : EntityModel {
			Register(descriptor, modelFactory, EntityRenderer.IFactory.Of(rendererFactory));
		}

		private static void Register(EntityDescriptor descriptor, EntityModel.Factory modelFactory, EntityRenderer.IFactory rendererFactory) {
			MODEL_FACTORIES.Add(descriptor, modelFactory);
			RENDERER_FACTORIES.Add(descriptor, rendererFactory);
		}

		public static Func<EntityDescriptor, EntityModel> GetModelSupplier() {
			return descriptor => MODEL_FACTORIES.GetValueOrDefault(descriptor, () => new MissingEntityModel())();
		}

		public static Dictionary<EntityDescriptor, EntityRenderer> GetRenderers(List<EntityDescriptor> descriptors, EntityRenderer.FactoryContext context) {
			Dictionary<EntityDescriptor, EntityRenderer> renderers = new();
			foreach (var descriptor in descriptors) {
				EntityRenderer.IFactory factory = RENDERER_FACTORIES.GetValueOrDefault(
					descriptor,
					EntityRenderer.IFactory.Of(context => {
						Logger.LogError("Renderer not found: {}", descriptor);
						return new EmptyEntityRenderer<Entity>(context);
					})
				);
				renderers[descriptor] = factory.GetRenderer(context);
			}
			return renderers;
		}
	}
}
