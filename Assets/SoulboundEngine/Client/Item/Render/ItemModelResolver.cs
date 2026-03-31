namespace SoulboundEngine.Client.ItemSystem.Render {
	public sealed class ItemModelResolver {
		public ItemRenderModel Resolve(ItemRenderData renderData) {
			return new ItemRenderModel {
				showStackText = renderData.isStackable,
				spriteRef = renderData.spriteRef,
				stackQuantity = renderData.stackQuantity,
				spriteAnimation = renderData.spriteAnimation,
			};
		}
	}
}
