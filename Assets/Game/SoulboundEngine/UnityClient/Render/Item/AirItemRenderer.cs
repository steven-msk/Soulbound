using SoulboundEngine.Item;

namespace SoulboundEngine.UnityClient.Render.Item {
	public sealed class AirItemRenderer : ItemRenderer<ItemRenderState> {
		public override ItemRenderState CreateRenderState(ItemStack stack, ItemRenderContext context) => new();

		public override ItemViewHandle Create(ItemRenderState state, ItemModel model, ItemRenderContext context) {
			return ItemViewHandle.Of((UnityEngine.GameObject)null);
		}

		public override void Destroy(ItemViewHandle view, ItemRenderContext context) {
		}

		public override void Update(ItemRenderState state, ItemViewHandle view, ItemRenderContext context) {
		}
	}
}
