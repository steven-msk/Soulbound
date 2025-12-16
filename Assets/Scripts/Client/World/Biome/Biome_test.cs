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
		const float tunnelThreshold = 0.92f;
		const float tunneKillThreshold = tunnelThreshold + 0.03f;
		const float verticalTunnelCompression = 0.2f;

		private readonly int platformHeight;
		private readonly int seed;
		private readonly Heightmap heightmap;
		private readonly PerlinNoise largeNoise;
		private readonly PerlinNoise mediumNoise;
		private readonly PerlinNoise detailNoise;
		private readonly PerlinNoise smallCaveNoise;
		private readonly PerlinNoise tunnelKillNoise;
		private readonly DomainWarp warp;

		public Biome_test(int seed, int platformHeight) {
			this.platformHeight = platformHeight;
			this.largeNoise = new PerlinNoise(1, seed, frequency: 1f, amplitude: 60f);
			this.mediumNoise = new PerlinNoise(2, seed, frequency: 0.7f, amplitude: 40f);
			this.detailNoise = new PerlinNoise(3, seed, frequency: 0.12f, amplitude: 5f);
			this.heightmap = new Heightmap(platformHeight);
			this.smallCaveNoise = new PerlinNoise(4, seed, frequency: 0.3f, amplitude: 1f);
			this.tunnelKillNoise = new PerlinNoise(5, seed, frequency: 0.25f, amplitude: 1.76f);
			this.warp = new DomainWarp(seed, frequency: 0.15f);   
		}

		public float GetDepth(int x, int y) {
			float height = SurfaceDepthAtX(x);
			float depth = height - y;
			float normalized = Mathf.Clamp01(depth / maxSolidDepth);
			return normalized * maxSolidDepth;
		}

		private bool IsCave(int x, int y) {
			float n = Mathf.Abs(smallCaveNoise.Sample2D(x, y));
			float mask = Mathf.Pow(GetVerticalMask(x, y), 4f);

			float thresholdFactor = Mathf.InverseLerp(WorldChunk.minY, SurfaceDepthAtX(x) - surfaceFalloff, y);
			thresholdFactor = FastExp(1f - Mathf.Clamp01(thresholdFactor), 20);

			return n * mask > 1f - thresholdFactor;
		}

		private bool IsTunnel(int x, int y) {
			float wx = x, wy = y, zSlice = seed;
			float verticalMask = GetVerticalMask(x, y);

			warp.Warp3D(ref wx, ref wy, ref zSlice);
			float n = 1f - Mathf.Abs(smallCaveNoise.Sample2D(wx, wy / verticalTunnelCompression));
			n *= verticalMask;

			return n > tunnelThreshold && !TunnelKill(x, y);
		}

		private bool TunnelKill(int x, int y) {
			float k = Mathf.Abs(tunnelKillNoise.Sample2D(x, y / verticalTunnelCompression));
			float killMask = Mathf.Lerp(tunnelThreshold, 1f, k * k);
			return killMask > tunneKillThreshold;
		}

		private float GetPeakBias(int x, int y) {
			float verticalMask = GetVerticalMask(x, y);
			float relativeDepth = SurfaceDepthAtX(x) - y;
			const float minCarveDepth = 0.6f * maxSolidDepth;
			const float maxCarveDepth = 0.98f * maxSolidDepth;
			float depthFactor = Mathf.InverseLerp(minCarveDepth, maxCarveDepth, relativeDepth);
			return Mathf.Clamp01(depthFactor);
		}

		private float SurfaceDepthAtX(int x) {
			float ln = Mathf.Abs(largeNoise.Sample1D(x));
			float mn = Mathf.Abs(mediumNoise.Sample1D(x));
			float dn = Mathf.Abs(detailNoise.Sample1D(x));
			return platformHeight + ln + mn + dn;
		}

		private int SurfaceHeightAtX(int x) {
			return Mathf.FloorToInt(SurfaceDepthAtX(x));
		}

		private float GetSurfaceMask(int x, int y, float falloff) {
			float surfaceY = SurfaceDepthAtX(x);
			float t = Mathf.InverseLerp(surfaceY - falloff, surfaceY, y);
			return 1f - Mathf.Clamp01(Mathf.SmoothStep(0f, 1f, t));
		}

		private float GetBottomMask(int y) {
			float t = Mathf.InverseLerp(WorldChunk.minY, WorldChunk.minY + bottomFalloff, y);
			return Mathf.Clamp01(Mathf.SmoothStep(0f, 1f, t));
		}

		private float GetVerticalMask(int x, int y) {
			return GetSurfaceMask(x, y, surfaceFalloff) * GetBottomMask(y);
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

		public void PlaceFeatures(WorldChunk chunk, Level level) {
			for (int cx = 0; cx < Level.CHUNK_LENGTH; cx++) {
				int x = chunk.ChunkXToWorldX(cx);
				int y = SurfaceHeightAtX(x) + 1;

				ChunkBlockPos pos = new(cx, y, chunk.xpos);
				chunk.SetBlock(pos, Blocks.leaves.defaultState);
			}
		}

		private static float FastExp(float b, int n) {
			float p = 1f;
			while (n > 0) {
				if (n % 2 == 1) {
					p *= b;
				}
				b *= b;
				n /= 2;
			}
			return p;
		}
	}
}
