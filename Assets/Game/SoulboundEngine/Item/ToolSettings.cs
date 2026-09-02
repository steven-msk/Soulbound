namespace SoulboundEngine.Item {
	using SoulboundEngine.Registry;
	using System.Collections.Generic;

	public record ToolSettings(ToolType toolType, int durability, float speed) {

		private Item.Settings ApplyCommonSettings(Item.Settings settings) {
			return settings.Durability(this.durability);
		}

		public Item.Settings Apply(Item.Settings settings) {
			return this.ApplyCommonSettings(settings)
				.Component(ItemComponents.TOOL, new Tool(
					new List<Tool.Rule>() {
						Tool.Rule.Mines(this.toolType.GetBlocksThatMines(Registries.BLOCKS), this.speed),
						Tool.Rule.CantMine(this.toolType.GetBlocksThatCantMine(Registries.BLOCKS))
					},
					damagePerBlock: 1
				)
			);
		}

	}
}
