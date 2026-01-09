using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.Generation {
	public record NoiseSettings {
		public FastNoiseLite.NoiseType noiseType = FastNoiseLite.NoiseType.Value;
		public FastNoiseLite.DomainWarpType domainWarpType = FastNoiseLite.DomainWarpType.OpenSimplex2;
		public FastNoiseLite.FractalType fractalType = FastNoiseLite.FractalType.None;
		public FastNoiseLite.RotationType3D rotationType3D = FastNoiseLite.RotationType3D.None;
		public FastNoiseLite.CellularDistanceFunction cellularDistanceFunction = FastNoiseLite.CellularDistanceFunction.Euclidean;
		public FastNoiseLite.CellularReturnType cellularReturnType = FastNoiseLite.CellularReturnType.CellValue;
		public float frequency = 0.01f;
		public float domainWarpAmp = 1.0f;
		public float fractalGain = 0.5f;
		public float fractalLacunarity = 2.0f;
		public int fractalOctaves = 3;
		public float fractalPingPingStrength = 2.0f;
		public float fractalWeightedStrength = 0.0f;
		public float cellularJitter = 0.0f;

		public NoiseSettings() { }

		public NoiseSettings(FastNoiseLite.NoiseType noiseType, float frequency) {
			this.noiseType = noiseType;
			this.frequency = frequency;
		}
	}
}
