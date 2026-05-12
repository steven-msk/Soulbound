using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.Render.Item {
	public abstract class ItemRenderContext {

		public sealed class GUI : ItemRenderContext {
			public RectTransform? parent;
		}

		public sealed class World : ItemRenderContext {
			public Vector3 position;
		}
	}
}
