using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Client.World.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Client.World.Biome {
	public class Biome_test : IBiome {
		const float maxSolidDepth = 10f;
		const float surfaceFalloff = 15f;
		const float bottomFalloff = 10f;
		const float caveThreshold = 0.98f;
		const float tunnelThreshold = 0.95f;

		private readonly int platformHeight;
		private readonly int seed;
		private readonly Heightmap heightmap;
		private readonly PerlinNoise largeNoise;
		private readonly PerlinNoise mediumNoise;
		private readonly PerlinNoise detailNoise;
		private readonly PerlinNoise caveNoise;
		private readonly DomainWarp warp;
		private readonly DomainWarp largeWarp;

		public Biome_test(int seed, int platformHeight) {
			this.platformHeight = platformHeight;
			this.largeNoise = new PerlinNoise(seed, frequency: 1f, amplitude: 60f);
			this.mediumNoise = new PerlinNoise(seed, frequency: 0.7f, amplitude: 40f);
			this.detailNoise = new PerlinNoise(seed, frequency: 0.12f, amplitude: 5f);
			this.heightmap = new Heightmap(platformHeight);
			this.caveNoise = new PerlinNoise(seed, frequency: 0.5f, amplitude: 1f);
			this.warp = new DomainWarp(seed, frequency: 0.125f);
		}

		public float GetDepth(int x, int y) {
			float height = SurfaceHeightAtX(x);
			float depth = height - y;
			float normalized = Mathf.Clamp01(depth / maxSolidDepth);
			return normalized * maxSolidDepth;
		}

		private bool IsCave(int x, int y) {
			float n = Mathf.Abs(caveNoise.Sample2D(x, y));
			return n * GetVerticalMask(x, y) > caveThreshold;
		}

		private bool IsTunnel(int x, int y) {
			float wx = x, wy = y;
			warp.Warp2D(ref wx, ref wy);
			float n = 1f - Mathf.Abs(caveNoise.Sample2D(x + wx, y + wy));

			float verticalMask = GetVerticalMask(x, y) * GetPeakBias(x, y);
			return n * verticalMask > tunnelThreshold;
		}

		private float GetPeakBias(int x, int y) {
			float verticalMask = GetVerticalMask(x, y);
			float relativeDepth = SurfaceHeightAtX(x) - y;
			const float minCarveDepth = 0.6f * maxSolidDepth;
			const float maxCarveDepth = 0.98f * maxSolidDepth;
			float depthFactor = Mathf.InverseLerp(minCarveDepth, maxCarveDepth, relativeDepth);
			return Mathf.Clamp01(depthFactor);
		}

		private float SurfaceHeightAtX(int x) {
			float ln = Mathf.Abs(largeNoise.Sample1D(x));
			float mn = Mathf.Abs(mediumNoise.Sample1D(x));
			float dn = Mathf.Abs(detailNoise.Sample1D(x));
			return platformHeight + ln + mn + dn;
		}

		private float GetSurfaceMask(int x, int y) {
			float surfaceY = SurfaceHeightAtX(x);
			float t = Mathf.InverseLerp(surfaceY - surfaceFalloff, surfaceY, y);
			return 1f - Mathf.Clamp01(Mathf.SmoothStep(0f, 1f, t));
		}

		private float GetBottomMask(int y) {
			float t = Mathf.InverseLerp(WorldChunk.minY, WorldChunk.minY + bottomFalloff, y);
			return Mathf.Clamp01(Mathf.SmoothStep(0f, 1f, t));
		}

		private float GetVerticalMask(int x, int y) {
			return GetSurfaceMask(x, y) * GetBottomMask(y);
		}

		public BlockState ResolveBlock(float depth, int x, int y) {
			if (depth <= 0 || IsCave(x, y) || IsTunnel(x, y))
				return Blocks.air.defaultState;
			if (depth < 4)
				return Blocks.grass.defaultState;
			if (depth < 1)
				return Blocks.dirt.defaultState;
			return Blocks.stone.defaultState;
		}
	}
}
