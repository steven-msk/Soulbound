
using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Client.World.BlockSystem.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.World.Generation {
	public sealed class BlockResolver {
		const int blendRange = 10;

		private readonly IBiome primary;
		private readonly IBiome? secondary;

		public BlockResolver(IBiome primary, IBiome? secondary) {
			this.primary = primary;
			this.secondary = secondary;
		}

		public BlockState ResolveBlock(BlockGenContext ctx) {
			return ResolveBlock(ctx, primary);
		}

		private BlockState ResolveBlock(BlockGenContext ctx, IBiome biome) {
			return ctx.isCave
				? ResolveCaveBlock(ctx.pos, ctx.caveDensity, biome)
				: ResolveTerrainBlock(ctx, biome);
		}

		private BlockState ResolveTerrainBlock(BlockGenContext ctx, IBiome biome) {
			return biome.ResolveBlock(ctx);
		}

		private BlockState ResolveCaveBlock(BlockPos pos, float caveDensity, IBiome biome) {
			return biome.ResolveCaveBlock(pos, caveDensity);
		}

		public BlockState BlendBiomeBorder(BlockGenContext ctx, int leftX, int rightX) {
			float t = Mathf.InverseLerp(leftX, rightX, ctx.pos.x);
			t = Mathf.Pow(t, 1.7f) + UnityEngine.Random.value;

			return t > 0f && t < 1f
				? ResolveBlock(ctx, primary)
				: ResolveBlock(ctx, secondary);
		}
	}
}
