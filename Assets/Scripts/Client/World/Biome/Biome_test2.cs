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
	public class Biome_test2 : IBiome {
		const float maxSolidDepth = 10f;
		const float surfaceFalloff = 15f;
		const float bottomFalloff = 10f;
		const float caveThreshold = 0.9f;
		const float tunnelThreshold = 0.95f;
		const float tunneKillThreshold = tunnelThreshold + 0.03f;
		const float verticalTunnelCompression = 1f;
		const int platformHeight = -50;

		private readonly int seed;
		private readonly Heightmap heightmap;
		private readonly PerlinNoise largeNoise;
		private readonly PerlinNoise mediumNoise;
		private readonly PerlinNoise detailNoise;
		private readonly PerlinNoise caveNoise;
		private readonly PerlinNoise tunnelKillNoise;
		private readonly DomainWarp warp;
		private readonly PerlinNoise forestNoise;
		private readonly PerlinNoise forestDensityNoise;
		private readonly PerlinNoise densityNoise;
		int lastTreeX = int.MinValue >> 1;

		public Biome_test2(int seed) {
			this.largeNoise = new PerlinNoise(1, seed, frequency: 1f, amplitude: 60f);
			this.mediumNoise = new PerlinNoise(2, seed, frequency: 0.7f, amplitude: 40f);
			this.detailNoise = new PerlinNoise(3, seed, frequency: 0.12f, amplitude: 5f);
			this.heightmap = new Heightmap(platformHeight);
			this.caveNoise = new PerlinNoise(4, seed, frequency: 0.5f, amplitude: 1.5f);
			this.tunnelKillNoise = new PerlinNoise(5, seed, frequency: 0.25f, amplitude: 1.76f);
			this.warp = new DomainWarp(seed, frequency: 0.15f);
			this.forestNoise = new PerlinNoise(6, seed, frequency: 3f, amplitude: 10f);
			this.forestDensityNoise = new PerlinNoise(7, seed, frequency: 5f, amplitude: 4f);
			this.densityNoise = new PerlinNoise(8, seed, frequency: 0.06f, amplitude: 1f);
		}

		float IBiome.GetDensity(BlockPos pos) {
			const float threshold = 1f;
			float n = Mathf.Max(0f, densityNoise.Sample1D(pos.x));
			n = Mathf.Pow(n, 6f);
			//n = Mathf.Max(0f, n - threshold);
			return n;
		}

		float IBiome.GetDepth(BlockPos pos) {
			float height = SurfaceDepthAtX(pos.x);
			float depth = height - pos.y;
			float normalized = Mathf.Clamp01(depth / maxSolidDepth);
			return normalized * maxSolidDepth;
		}

		private bool IsCave(int x, int y) {
			float n = Mathf.Abs(caveNoise.Sample2D(x, y));
			float mask = GetVerticalMask(x, y);

			float f = Mathf.InverseLerp(WorldChunk.minY, WorldChunk.maxY, y);
			float depthFactor = 1f - Mathf.Clamp01(Mathf.SmoothStep(0f, 1f, f));

			return n * mask > caveThreshold / depthFactor;
		}

		private bool IsTunnel(int x, int y) {
			float wx = x, wy = y, zSlice = seed;
			float verticalMask = GetVerticalMask(x, y);

			warp.Warp3D(ref wx, ref wy, ref zSlice);
			float n = 1f - Mathf.Abs(caveNoise.Sample2D(wx, wy / verticalTunnelCompression));
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

		public BlockState ResolveBlock(float depth, BlockPos pos) {
			return Blocks.dirt.defaultState;
		}

		public void TryPlaceFeature(int cx, WorldChunk chunk, Level level) {
		}
	}
}
