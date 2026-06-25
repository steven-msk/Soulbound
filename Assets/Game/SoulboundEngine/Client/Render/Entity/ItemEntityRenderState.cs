using SoulboundEngine.Client.Item;
using SoulboundEngine.Client.World.Entity;

namespace SoulboundEngine.Client.Render.Entity {
	public sealed class ItemEntityRenderState : EntityRenderState<ItemEntity> {
		public ItemStack stack;
	}
}
