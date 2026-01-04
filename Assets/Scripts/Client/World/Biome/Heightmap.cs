using Assets.Scripts.Client.World.Biome;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.Chunk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client.World.Generation {
	public sealed class Heightmap {
		public int planeY { get; private set; }
		public int planeHeight => WorldChunk.maxY - planeY;

		public Heightmap(int planeY) {
			this.planeY = planeY;
		}

		public float SampleHeight(int blockX, BiomeMap biomeMap) {
			float baseHeight = planeHeight;
			var weights = biomeMap.ResolveWeights(blockX)
				.OrderByDescending(w => w.value)
				.ToList();
			if (weights.Count == 0) {
				return planeHeight;
			}

			var primary = weights[0];
			BiomeWeight? secondary = weights.Count > 1 ? weights[1] : null;

			var w1 = primary.value;
			var w2 = secondary.GetValueOrDefault().value;
			float t = secondary != null ? w2 / (w1 + w2) : 0f;

			var a = primary.biome.SampleTerrain(blockX);
			if (secondary == null || t < 0.001f) {
				return ApplyModulation(a);
			}
			var b = secondary.Value.biome.SampleTerrain(blockX);

			var blended = new TerrainModulation {
				heightOffset = Mathf.Lerp(a.heightOffset, b.heightOffset, t),
				amplitude = Mathf.Lerp(a.amplitude, b.amplitude, t),
				erosion = Mathf.Lerp(a.erosion, b.erosion, t)
			};

			return ApplyModulation(blended);
		}

		public float ApplyModulation(TerrainModulation m) {
			float baseHeight = planeHeight + m.heightOffset;
			float variation = (planeHeight * (m.amplitude - 1f));
			variation *= m.erosion;
			return baseHeight + variation;
		}
	}
}
