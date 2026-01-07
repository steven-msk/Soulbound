using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Client.World.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using TerrainData = SoulboundBackend.Client.World.Generation.TerrainData;

namespace Assets.Scripts.Client.World.Biome {
	public class PlainsBiome_test : IBiome {
		const float maxSolidDepth = 10f;
		const float surfaceFalloff = 15f;
		const float bottomFalloff = 10f;
		const float caveThreshold = 0.7f;
		const float tunnelThreshold = 0.92f;
		const float tunneKillThreshold = tunnelThreshold + 0.03f;
		const float verticalTunnelCompression = 0.2f;
		const int platformHeight = 0;

		private readonly int seed;
		private readonly PerlinNoise largeNoise;
		private readonly PerlinNoise mediumNoise;
		private readonly PerlinNoise detailNoise;
		[Obsolete] private readonly PerlinNoise caveNoise;
		[Obsolete] private readonly PerlinNoise tunnelKillNoise;
		private readonly DomainWarp warp;
		private readonly PerlinNoise densityNoise;
		int lastTreeX = int.MinValue >> 1;

		public PlainsBiome_test(int seed) {
			this.largeNoise = new PerlinNoise(1, seed, frequency: 0.3f, amplitude: 30f);
			this.mediumNoise = new PerlinNoise(2, seed, frequency: 0.1f, amplitude: 20f);
			this.detailNoise = new PerlinNoise(3, seed, frequency: 0.02f, amplitude: 5f);

			this.caveNoise = new PerlinNoise(4, seed, frequency: 0.5f, amplitude: 1.5f);
			this.tunnelKillNoise = new PerlinNoise(5, seed, frequency: 0.25f, amplitude: 1.76f);
			this.warp = new DomainWarp(seed, frequency: 0.15f);
			this.densityNoise = new PerlinNoise(8, seed, frequency: 0.05f, amplitude: 1f);
		}

		float IBiome.GetDensity(int blockX) {
			float n = Mathf.Abs(densityNoise.Sample1D(blockX));
			n = Mathf.Pow(n, 1.5f);
			return n;
		}

		private bool IsCave(int x, int y, BlockContext ctx) {
			float n = Mathf.Abs(caveNoise.Sample2D(x, y));
			float mask = GetVerticalMask(x, y, ctx);

			float f = Mathf.InverseLerp(WorldChunk.minY, WorldChunk.maxY, y);
			float depthFactor = 1f - Mathf.Clamp01(Mathf.SmoothStep(0f, 1f, f));

			return n * mask > caveThreshold / depthFactor;
		}

		private bool IsTunnel(int x, int y, BlockContext ctx) {
			float wx = x, wy = y, zSlice = seed;
			float verticalMask = GetVerticalMask(x, y, ctx);

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

		private float HeightNoise(int x) {
			float ln = Mathf.Abs(largeNoise.Sample1D(x));
			float mn = Mathf.Abs(mediumNoise.Sample1D(x));
			float dn = Mathf.Abs(detailNoise.Sample1D(x));
			return ln + mn + dn;
		}

		private float GetSurfaceMask(int x, int y, float falloff, BlockContext ctx) {
			float t = Mathf.InverseLerp(ctx.surfaceY - falloff, ctx.surfaceY, y);
			return 1f - Mathf.Clamp01(Mathf.SmoothStep(0f, 1f, t));
		}

		private float GetBottomMask(int y) {
			float t = Mathf.InverseLerp(WorldChunk.minY, WorldChunk.minY + bottomFalloff, y);
			return Mathf.Clamp01(Mathf.SmoothStep(0f, 1f, t));
		}

		private float GetVerticalMask(int x, int y, BlockContext ctx) {
			return GetSurfaceMask(x, y, surfaceFalloff, ctx) * GetBottomMask(y);
		}

		BlockState IBiome.ResolveBlock(BlockContext ctx) {
			if (ctx.AboveSurface())
				return Blocks.air.defaultState;
			return Blocks.dirt.defaultState;
		}

		TerrainModulation IBiome.SampleTerrain(int blockX) {
			return new TerrainModulation {
				heightOffset = 30 + HeightNoise(blockX),
				amplitude = 0.35f,
				erosion = 0.85f
			};
		}

		CaveModulation IBiome.SampleCave(int blockX, int blockY) {
			return new CaveModulation {
				frequency = 1f,
				edgeSharpness = 1.5f,
				fill = 0.5f,
				surfaceFalloff = 30f
			};
		}
	}
}
