namespace SoulboundEngine.Item {
	using SoulboundEngine.Registry;
	using System.Collections.Generic;

	public record ToolSettings(ToolPower toolType, int durability, float speed, int durabilityCost = 1) {

		private Item.Settings ApplyCommonSettings(Item.Settings settings) {
			return settings.NonStackable().Durability(this.durability);
		}

		public Item.Settings Apply(Item.Settings settings) {
			return this.ApplyCommonSettings(settings)
				.Component(ItemComponents.TOOL, new Tool(
					new List<Tool.Rule>() {
						Tool.Rule.Mines(this.toolType.GetBlocksThatMines(Registries.BLOCKS), this.speed),
						Tool.Rule.CantMine(this.toolType.GetBlocksThatCantMine(Registries.BLOCKS))
					},
					this.durabilityCost
				)
			);
		}

	}
}
