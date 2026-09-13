namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Block.State;
	using SoulboundEngine.World.Chunk;
	using SoulboundEngine.World.Level;

#nullable enable

	public class SurfaceBuilder {
		private readonly BlockState defaultBlock;

		public SurfaceBuilder(BlockState defaultBlock) {
			this.defaultBlock = defaultBlock;
		}

		public void BuildSurface(Chunk chunk, SurfaceRules.IRuleSource ruleSource) {
			SurfaceRules.Context context = new(x => chunk.GetBiome(chunk.GetPos().ToLocalX(x)));
			SurfaceRules.ISurfaceRule rule = ruleSource.Apply(context);
			Heightmap heightmap = chunk.GetHeightmap();
			BlockPos.Mutable blockPos = new();

			for (int x = 0; x < Level.CHUNK_LENGTH; x++) {
				int worldX = chunk.GetPos().ToWorldX(x);
				int surfaceY = heightmap.GetFirstFree(x);
				int endY = chunk.GetBottomY();
				context.UpdateX(worldX);

				int stoneAboveDepth = 0;
				for (int y = surfaceY; y >= endY; y--) {
					blockPos.Set(worldX, y);
					BlockState current = chunk.GetBlockState(blockPos);
					if (current.IsAir()) {
						stoneAboveDepth = 0;
						continue;
					}

					stoneAboveDepth++;
					context.UpdateY(y, stoneAboveDepth);

					if (current == this.defaultBlock) {
						BlockState? replacement = rule.TryApply(worldX, y);
						if (replacement != null) {
							chunk.SetBlockState(blockPos, replacement);
						}
					}
				}
			}
		}
	}
}
