using SoulboundEngine.Item;
using SoulboundEngine.World.Entity;

namespace SoulboundEngine.Client.Render.Entity {
	public sealed class ItemEntityRenderState : EntityRenderState<ItemEntity> {
		public ItemStack stack;
	}
}
