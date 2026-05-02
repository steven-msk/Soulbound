using SoulboundEngine.Client.World.EntitySystem;
using System;
using System.Collections.Generic;

namespace SoulboundEngine.Client.Render.Entity {
	using Entity = World.EntitySystem.Entity;

	public static class EntityRenderers {
		private static readonly Dictionary<EntityDescriptor, IEntityModelResolver.Factory> MODEL_FACTORIES = new();
		private static readonly Dictionary<EntityDescriptor, EntityRenderer.Factory> RENDERER_FACTORIES = new();

		public static void Register(EntityDescriptor descriptor, IEntityModelResolver.Factory modelFactory, EntityRenderer.Factory rendererFactory) {
			MODEL_FACTORIES.Add(descriptor, modelFactory);
			RENDERER_FACTORIES.Add(descriptor, rendererFactory);
		}

		public static Func<EntityDescriptor, IEntityModelResolver> GetModelResolverFactory() {
			return descriptor => MODEL_FACTORIES.GetValueOrDefault(descriptor, () => new IEntityModelResolver.Missing())();
		}

		public static Dictionary<EntityDescriptor, EntityRenderer> GetRenderers(List<EntityDescriptor> descriptors) {
			Dictionary<EntityDescriptor, EntityRenderer> renderers = new();
			foreach (var descriptor in descriptors) {
				renderers[descriptor] = RENDERER_FACTORIES.GetValueOrDefault(descriptor, () => new EmptyEntityRenderer<Entity>())();
			}
			return renderers;
		}
	}
}
