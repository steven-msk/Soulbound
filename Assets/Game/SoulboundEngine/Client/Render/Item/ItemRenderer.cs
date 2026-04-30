using SoulboundEngine.Client.ItemSystem;

namespace SoulboundEngine.Client.Render.Items {
	public abstract class ItemRenderer {
		public const float IMAGE_SIZE = 32f;
		public const float STACK_TEXT_SIZE = 8f;

		public delegate ItemRenderer Factory();
	}

	public abstract class ItemRenderer<I, S> : ItemRenderer where I : Item where S : ItemRenderState<I> {
		public abstract void CreateView(S state, ItemRenderContext context);
		public abstract void UpdateView(S state, ItemRenderContext context);
		public abstract void DestroyView(S state);

		public abstract S CreateRenderState();
	}
}
