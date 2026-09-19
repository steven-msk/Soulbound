namespace SoulboundEngine.World.Gen.Surface {
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Block.State;
	using SoulboundEngine.World.Chunk;
	using SoulboundEngine.World.Gen.Function;
	using SoulboundEngine.World.Level;
	using System;

#nullable enable

	public class SurfaceBuilder {
		private readonly BlockState defaultBlock;

		public SurfaceBuilder(BlockState defaultBlock) {
			this.defaultBlock = defaultBlock;
		}

		public void BuildSurface(RandomState randomState, Chunk chunk, SurfaceRules.IRuleSource ruleSource) {
			SurfaceRules.Context context = new(chunk, x => chunk.GetBiome(chunk.pos.ToLocalX(x)));
			IDensityFunction terrainHeight = randomState.Router.terrainHeight;
			SurfaceRules.ISurfaceRule rule = ruleSource.Apply(context);
			Heightmap heightmap = chunk.GetHeightmap();
			BlockPos.Mutable blockPos = new();

			const int SURFACE_TOLERANCE = 5;

			for (int x = 0; x < Level.CHUNK_LENGTH; x++) {
				int worldX = chunk.pos.ToWorldX(x);
				int surfaceY = heightmap.GetFirstFree(x);
				int endY = chunk.GetBottomY();
				context.UpdateX(worldX);

				int analyticSurfaceY = (int)Math.Round(terrainHeight.Compute(new IDensityFunction.SinglePointContext(worldX, surfaceY)));
				int stoneAboveDepth = 0;
				for (int y = surfaceY; y >= endY; y--) {
					blockPos.Set(worldX, y);
					BlockState current = chunk.GetBlockState(blockPos);
					if (current.IsAir()) continue;

					stoneAboveDepth++;
					if (analyticSurfaceY - y > SURFACE_TOLERANCE) continue;

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
