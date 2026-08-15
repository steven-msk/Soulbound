namespace SoulboundEngine.World.Block {
	using Level = Level.Level;

	public interface INeighborUpdateHandler {
		void OnNeighborChanged(Level level, BlockPos selfPos, BlockPos neighborPos);
	}
}
