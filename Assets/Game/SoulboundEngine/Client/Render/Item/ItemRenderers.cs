using SoulboundEngine.Core.Registry;
using System.Collections.Generic;

namespace SoulboundEngine.Client.Render.Items {
	public static class ItemRenderers {
		private static readonly Dictionary<Identifier, IItemModelResolver> MODEL_RESOLVERS = new();
		private static readonly Dictionary<Identifier, ItemRenderer.Factory> RENDERERS = new();

	}
}
