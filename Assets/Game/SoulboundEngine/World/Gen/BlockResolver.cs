#nullable enable

namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.Common.Math.Random;
	using SoulboundEngine.World.Biome;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Block.State;
	using System;

	public sealed class BlockResolver {
		const int blendRange = 10;
		private readonly IRandom random = RandomProvider.CreateWithUniqueSeed();
		private readonly IBiome primary;
		private readonly IBiome? secondary;

		public BlockResolver(IBiome primary, IBiome? secondary) {
			this.primary = primary;
			this.secondary = secondary;
		}

		public BlockState ResolveBlock(BlockGenContext ctx) {
			return this.ResolveBlock(ctx, this.primary);
		}

		private BlockState ResolveBlock(BlockGenContext ctx, IBiome biome) {
			return ctx.isCave
				? this.ResolveCaveBlock(ctx.pos, ctx.caveDensity, biome)
				: this.ResolveTerrainBlock(ctx, biome);
		}

		private BlockState ResolveTerrainBlock(BlockGenContext ctx, IBiome biome) {
			return biome.ResolveBlock(ctx);
		}

		private BlockState ResolveCaveBlock(BlockPos pos, float caveDensity, IBiome biome) {
			return biome.ResolveCaveBlock(pos, caveDensity);
		}

		public BlockState BlendBiomeBorder(BlockGenContext ctx, int leftX, int rightX) {
			float t = (float)Maths.InverseLerp(leftX, rightX, ctx.pos.x);
			t = (float)Math.Pow(t, 1.7f) + this.random.NextFloat();

			return t > 0f && t < 1f
				? this.ResolveBlock(ctx, this.primary)
				: this.ResolveBlock(ctx, this.secondary);
		}
	}
}
