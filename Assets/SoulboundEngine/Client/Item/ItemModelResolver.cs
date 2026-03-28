namespace SoulboundEngine.Client.ItemSystem.Render {
	public sealed class ItemModelResolver {
		public ItemRenderModel Resolve(ItemRenderData renderData) {
			return new ItemRenderModel {
				showStackText = renderData.isStackable,
				spriteKey = renderData.spriteKey,
				stackQuantity = renderData.stackQuantity
			};
		}
	}
}
