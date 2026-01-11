using Assets.Scripts.Client.World.Biome;
using SoulboundBackend.Client.World.BlockSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client.World.Generation {
	public sealed class BlockResolver {
		const int blendRange = 10;

		private readonly IBiome primary;
		private readonly IBiome? secondary;

		public BlockResolver(IBiome primary, IBiome? secondary) {
			this.primary = primary;
			this.secondary = secondary;
		}

		public BlockState ResolveBlock(BlockGenContext ctx) {
			return ctx.isCave
				? ResolveCaveBlock(ctx.pos, ctx.caveDensity)
				: ResolveTerrainBlock(ctx);
		}

		private BlockState ResolveTerrainBlock(BlockGenContext ctx) {
			return primary.ResolveBlock(ctx);
		}

		private BlockState ResolveCaveBlock(BlockPos pos, float caveDensity) {
			return primary.ResolveCaveBlock(pos, caveDensity);
		}

		//public BlockState ApplyBorderBlend(BlockPos pos, Func<IBiome, BlockState> stateFunction) {
		//	//return stateFunction(primary.biome);

		//	// previous implementation had severe flaws
		//	// reverted to primary biome selection for now

		//	if (secondary == null) {
		//		return stateFunction(primary.biome);
		//	}

		//	float w2 = secondary.Value.value;
		//	float t = Mathf.InverseLerp(0.8f, 1f, w2);

		//	if (t > 0f && t < 1f) {
		//		return stateFunction(secondary.Value.biome);
		//	}
		//	return stateFunction(primary.biome);
		//}

		public BlockState BlendBiomeBorder(BlockGenContext ctx, int leftX, int rightX, int splitX) {
			float t = Mathf.InverseLerp(leftX, rightX, ctx.pos.x);
			t = t * t + UnityEngine.Random.value;

			if (t > 0f && t < 1f) {
				return primary!.ResolveBlock(ctx);
			}
			return secondary.ResolveBlock(ctx);
		}
	}
}
