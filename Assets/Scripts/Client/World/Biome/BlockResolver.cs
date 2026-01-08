using SoulboundBackend.Client.World.BlockSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.World.Generation {
	public sealed class BlockResolver {
		private readonly BiomeMap biomeMap;
		private readonly BiomeWeight primary;
		private readonly BiomeWeight? secondary;
		private readonly FastNoiseLite borderWarp;

		public BlockResolver(BiomeMap biomeMap, BiomeWeight primary, BiomeWeight? secondary) {
			this.biomeMap = biomeMap;
			this.primary = primary;
			this.secondary = secondary;
			borderWarp = new FastNoiseLite();
			borderWarp.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
			borderWarp.SetFrequency(0.05f);
		}

		public BlockState ResolveBlock(BlockContext ctx) {
			return ctx.isCave
				? ResolveCaveBlock(ctx.pos, ctx.caveDensity)
				: ResolveTerrainBlock(ctx);
		}

		private BlockState ResolveTerrainBlock(BlockContext ctx) {
			return ApplyBorderBlend(ctx.pos, b => b.ResolveBlock(ctx));
		}

		private BlockState ResolveCaveBlock(BlockPos pos, float caveDensity) {
			return ApplyBorderBlend(pos, b => b.ResolveCaveBlock(pos, caveDensity));
		}

		private BlockState ApplyBorderBlend(BlockPos pos, Func<IBiome, BlockState> stateFunction) {
			if (secondary == null) {
				return stateFunction(primary.biome);
			}
			float w2 = secondary.Value.value;
			float v = Mathf.Pow(w2 - UnityEngine.Random.value, 5f);
			if (v < 0.1f) {
				return stateFunction(primary.biome);
			}
			return stateFunction(secondary.Value.biome);
		}
	}
}
