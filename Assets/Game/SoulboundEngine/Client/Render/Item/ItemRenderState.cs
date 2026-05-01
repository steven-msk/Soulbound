using SoulboundEngine.Client.ItemSystem;

namespace SoulboundEngine.Client.Render.Item {
	public class ItemRenderState<I> where I : ItemSystem.Item {
		public I item;
		public ItemStack stack;
		public ItemModel model;
		public bool showStackCount;
	}
}
