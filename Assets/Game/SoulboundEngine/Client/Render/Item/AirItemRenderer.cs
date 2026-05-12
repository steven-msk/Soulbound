using SoulboundEngine.Client.ItemSystem;

namespace SoulboundEngine.Client.Render.Item {
	public sealed class AirItemRenderer : ItemRenderer<ItemRenderState> {
		public override ItemRenderState CreateRenderState(ItemStack stack, ItemRenderContext context) => new();

		public override IItemView CreateView(ItemRenderState state, ItemModel model, ItemRenderContext context) => IItemView.Of(null);

		public override void DestroyView(IItemView view) {
		}

		public override void UpdateView(ItemRenderState state, IItemView view, ItemRenderContext context) {
		}
	}
}
