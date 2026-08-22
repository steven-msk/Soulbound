using SoulboundEngine.Item;
using SoulboundEngine.World.Entity;

namespace SoulboundEngine.UnityClient.Render.Entity {
	public sealed class ItemEntityRenderState : EntityRenderState<ItemEntity> {
		public ItemStack stack;
	}
}
