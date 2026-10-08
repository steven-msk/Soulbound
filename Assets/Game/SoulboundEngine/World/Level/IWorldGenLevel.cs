namespace SoulboundEngine.World.Level {
	using SoulboundEngine.World.Block;

	public interface IWorldGenLevel : ILevelAccess {
		long GetSeed();

		public virtual bool IsValidForSetBlock(BlockPos blockPos) => true;
	}
}
