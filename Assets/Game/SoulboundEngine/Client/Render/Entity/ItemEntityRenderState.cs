using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.World.EntitySystem;

namespace SoulboundEngine.Client.Render.Entity {
	public sealed class ItemEntityRenderState : EntityRenderState<ItemEntity> {
		public ItemStack stack;
	}
}
