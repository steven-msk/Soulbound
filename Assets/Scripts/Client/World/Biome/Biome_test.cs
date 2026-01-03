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
using UnityEngine;

namespace Assets.Scripts.Client.World.Biome {
	public class Biome_test : IBiome {
		const float maxSolidDepth = 10f;
		const float surfaceFalloff = 15f;
		const float bottomFalloff = 10f;
		const float caveThreshold = 0.7f;
		const float tunnelThreshold = 0.92f;
		const float tunneKillThreshold = tunnelThreshold + 0.03f;
		const float verticalTunnelCompression = 0.2f;
		const int platformHeight = 0;

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

		public Biome_test(int seed) {
			this.largeNoise = new PerlinNoise(1, seed, frequency: 1f, amplitude: 60f);
			this.mediumNoise = new PerlinNoise(2, seed, frequency: 0.7f, amplitude: 40f);
			this.detailNoise = new PerlinNoise(3, seed, frequency: 0.12f, amplitude: 5f);
			this.heightmap = new Heightmap(platformHeight);
			this.caveNoise = new PerlinNoise(4, seed, frequency: 0.5f, amplitude: 1.5f);
			this.tunnelKillNoise = new PerlinNoise(5, seed, frequency: 0.25f, amplitude: 1.76f);
			this.warp = new DomainWarp(seed, frequency: 0.15f);
			this.forestNoise = new PerlinNoise(6, seed, frequency: 3f, amplitude: 10f);
			this.forestDensityNoise = new PerlinNoise(7, seed, frequency: 5f, amplitude: 4f);
			this.densityNoise = new PerlinNoise(8, seed, frequency: 0.05f, amplitude: 1f);
		}

		float IBiome.GetDensity(int blockX) {
			float n = Mathf.Abs(densityNoise.Sample1D(blockX));
			n = Mathf.Pow(n, 6f);
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
			if (depth <= 0 || IsCave(pos.x, pos.y) || IsTunnel(pos.x, pos.y))
				return Blocks.air.defaultState;
			if (depth < 4)
				return Blocks.grass.defaultState;
			if (depth < 1)
				return Blocks.dirt.defaultState;
			return Blocks.stone.defaultState;
		}

		void PlaceTree(int originX, int originY, WorldChunk chunk, Level level) {
			const int crownRadius = 2;
			const int trunkHeightMin = 5;
			const int trunkHeightMax = 20;

			int chunkX = chunk.WorldXToChunkX(originX), ypos = originY;
			ChunkBlockPos trunkPos = new(chunkX, ypos, chunk.xpos);
			int height = UnityEngine.Random.Range(trunkHeightMin, trunkHeightMax + 1);

			for (int y = 0; y < height; y++) {
				chunk.SetBlock(trunkPos, Blocks.wood.defaultState);
				trunkPos.y++;
			}

			Dictionary<int, List<int>> rowToXs = new();
			float angularStep = 1f;
			for (float angle = 0; angle < 360f; angle += angularStep) {
				float rad = angle * Mathf.Deg2Rad;
				int x = Mathf.RoundToInt(trunkPos.x + crownRadius * Mathf.Cos(rad));
				int y = Mathf.RoundToInt(trunkPos.y + crownRadius * Mathf.Sin(rad));

				if (!rowToXs.ContainsKey(y)) {
					rowToXs[y] = new List<int>();
				}
				rowToXs[y].Add(x);
			}

			foreach (var kvp in rowToXs) {
				int y = kvp.Key;
				List<int> xs = kvp.Value;
				for (int x = xs.Min(); x <= xs.Max(); x++) {
					int cx = level.ChunkXFromRelativeBlock(x, chunk.xpos);
					ChunkBlockPos pos = new(level.NormalizeChunkX(x), y, cx);
					level.SetBlockOrPend(pos, Blocks.leaves.defaultState);
				}
			}

		}

		void IBiome.TryPlaceFeature(int cx, WorldChunk chunk, Level level) {
			const float forestThreshold = 0.45f;
			const float minTreeSpacing = 3;
			int xpos = chunk.ChunkXToWorldX(cx);
			int ypos = SurfaceHeightAtX(xpos) + 1;

			float forest = Mathf.Abs(forestNoise.Sample1D(xpos));
			float forestMask = Mathf.InverseLerp(forestThreshold, 1f, forest);
			if (forest < forestThreshold) {
				return;
			}
			float density = Mathf.Abs(forestDensityNoise.Sample1D(xpos));
			float distance = Mathf.Abs(xpos - lastTreeX);
			if (distance < minTreeSpacing) {
				return;
			}

			float spawnChance = Mathf.Lerp(0.05f, 0.25f, density);
			if(UnityEngine.Random.value < spawnChance) {
				PlaceTree(xpos, ypos, chunk, level);
				lastTreeX = xpos;
			}
		}
	}
}
