namespace SoulboundEngine.Client.Render.Item {
	using SoulboundEngine.Client.ItemSystem;
	using Item = ItemSystem.Item;

	public abstract class ItemRenderer {
		public const float IMAGE_SIZE = 32f;
		public const float STACK_TEXT_SIZE = 8f;

		public delegate ItemRenderer Factory();

		internal abstract object CreateRenderStateBoxed(ItemStack stack, ItemModel model);
		internal abstract IItemView CreateViewBoxed(object state, ItemRenderContext context);
		internal abstract void UpdateViewBoxed(object state, IItemView view, ItemRenderContext context);
		public abstract void DestroyView(IItemView view);
	}

	public abstract class ItemRenderer<I, S> : ItemRenderer where I : Item where S : ItemRenderState<I> {
		public abstract S CreateRenderState(ItemStack stack, ItemModel model);

		public abstract IItemView CreateView(S state, ItemRenderContext context);
		public abstract void UpdateView(S state, IItemView view, ItemRenderContext context);

		internal override object CreateRenderStateBoxed(ItemStack stack, ItemModel model) {
			return this.CreateRenderState(stack, model);
		}
		internal override IItemView CreateViewBoxed(object state, ItemRenderContext context) {
			return this.CreateView((S)state, context);
		}
		internal override void UpdateViewBoxed(object state, IItemView view, ItemRenderContext context) {
			this.UpdateView((S)state, view, context);
		}
	}
}
