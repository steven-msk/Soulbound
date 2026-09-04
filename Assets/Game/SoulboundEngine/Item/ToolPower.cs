namespace SoulboundEngine.Item {
	using SoulboundEngine.World.Block;
	using System.Collections.Generic;

	public enum ToolPower : int {
		NONE = 0,
		WOOD = 1,
		STONE = 2
	}

	public static class ToolPowerDefaults { 
		public static IEnumerable<Block> GetBlocksThatMines(this ToolPower power, IEnumerable<Block> blocks) {
			foreach (Block block in blocks) {
				if (power.CanMine(block)) yield return block;
			}
		}

		public static IEnumerable<Block> GetBlocksThatCantMine(this ToolPower power, IEnumerable<Block> blocks) {
			foreach (Block block in blocks) {
				if (!power.CanMine(block)) yield return block;
			}
		}

		public static bool CanMine(this ToolPower power, Block block) {
			return (int)power >= (int)block.GetRequiredToolPower();
		}
	}
}
