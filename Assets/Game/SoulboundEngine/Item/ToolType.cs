namespace SoulboundEngine.Item {
	using SoulboundEngine.World.Block;
	using System.Collections.Generic;

	public enum ToolType : int {
		NONE = 0,
		WOOD = 1,
		STONE = 2
	}

	public static class ToolTypeDefaults { 
		public static IEnumerable<Block> GetBlocksThatMines(this ToolType type, IEnumerable<Block> blocks) {
			foreach (Block block in blocks) {
				if (type.CanMine(block)) yield return block;
			}
		}

		public static IEnumerable<Block> GetBlocksThatCantMine(this ToolType type, IEnumerable<Block> blocks) {
			foreach (Block block in blocks) {
				if (!type.CanMine(block)) yield return block;
			}
		}

		public static bool CanMine(this ToolType type, Block block) {
			return (int)type >= (int)block.GetRequiredTool();
		}
	}
}
