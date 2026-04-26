using System;
using Vendor.FastNoiseLite;

namespace SoulboundEngine.Core.Noise {
	public sealed class FastNoiseLiteAdapter : INoise {
		private readonly FastNoiseLite fastNoiseLite = new();

		public FastNoiseLiteAdapter(int seed = 1337) {
			this.fastNoiseLite = new(seed);
		}

		public FastNoiseLiteAdapter(NoiseSettings settings) {
			settings.ApplyTo(this);
		}

		public void DomainWarp(ref float x, ref float y) {
			this.fastNoiseLite.DomainWarp(ref x, ref y);
		}

		public void DomainWarp(ref float x, ref float y, ref float z) {
			this.fastNoiseLite.DomainWarp(ref x, ref y, ref z);
		}

		public float GetNoise(float x, float y) {
			return this.fastNoiseLite.GetNoise(x, y);
		}

		public float GetNoise(float x, float y, float z) {
			return this.fastNoiseLite.GetNoise(x, y, z);
		}

		public void SetCellularDistanceFunction(CellularDistanceFunction cellularDistanceFunction) {
			this.fastNoiseLite.SetCellularDistanceFunction(ConvertCellularDistanceFunction(cellularDistanceFunction));
		}

		public void SetCellularJitter(float cellularJitter) {
			this.fastNoiseLite.SetCellularJitter(cellularJitter);
		}

		public void SetCellularReturnType(CellularReturnType cellularReturnType) {
			this.fastNoiseLite.SetCellularReturnType(ConvertCellularReturnType(cellularReturnType));
		}

		public void SetDomainWarpAmp(float domainWarpAmp) {
			this.fastNoiseLite.SetDomainWarpAmp(domainWarpAmp);
		}

		public void SetDomainWarpType(DomainWarpType domainWarpType) {
			this.fastNoiseLite.SetDomainWarpType(ConvertDomainWarpType(domainWarpType));
		}

		public void SetFractalGain(float gain) {
			this.fastNoiseLite.SetFractalGain(gain);
		}

		public void SetFractalLacunarity(float lacunarity) {
			this.fastNoiseLite.SetFractalLacunarity(lacunarity);
		}

		public void SetFractalOctaves(int octaves) {
			this.fastNoiseLite.SetFractalOctaves(octaves);
		}

		public void SetFractalPingPongStrength(float pingPongStrength) {
			this.fastNoiseLite.SetFractalPingPongStrength(pingPongStrength);
		}

		public void SetFractalType(FractalType fractalType) {
			this.fastNoiseLite.SetFractalType(ConvertFractalType(fractalType));
		}

		public void SetFractalWeightedStrength(float weightedStrength) {
			this.fastNoiseLite.SetFractalWeightedStrength(weightedStrength);
		}

		public void SetFrequency(float frequency) {
			this.fastNoiseLite.SetFrequency(frequency);
		}

		public void SetNoiseType(NoiseType noiseType) {
			this.fastNoiseLite.SetNoiseType(ConvertNoiseType(noiseType));
		}

		public void SetRotationType3D(RotationType3D rotationType3D) {
			this.fastNoiseLite.SetRotationType3D(ConvertRotationType3D(rotationType3D));
		}

		public void SetSeed(int seed) {
			this.fastNoiseLite.SetSeed(seed);
		}

		private static FastNoiseLite.CellularDistanceFunction ConvertCellularDistanceFunction(CellularDistanceFunction value) {
			return value switch {
				CellularDistanceFunction.Euclidean => FastNoiseLite.CellularDistanceFunction.Euclidean,
				CellularDistanceFunction.EuclideanSq => FastNoiseLite.CellularDistanceFunction.EuclideanSq,
				CellularDistanceFunction.Hybrid => FastNoiseLite.CellularDistanceFunction.Hybrid,
				CellularDistanceFunction.Manhattan => FastNoiseLite.CellularDistanceFunction.Manhattan,
				_ => throw new ArgumentException()
			};
		}

		private static FastNoiseLite.CellularReturnType ConvertCellularReturnType(CellularReturnType value) {
			return value switch {
				CellularReturnType.CellValue => FastNoiseLite.CellularReturnType.CellValue,
				CellularReturnType.Distance => FastNoiseLite.CellularReturnType.Distance,
				CellularReturnType.Distance2 => FastNoiseLite.CellularReturnType.Distance2,
				CellularReturnType.Distance2Add => FastNoiseLite.CellularReturnType.Distance2Add,
				CellularReturnType.Distance2Div => FastNoiseLite.CellularReturnType.Distance2Div,
				CellularReturnType.Distance2Mul => FastNoiseLite.CellularReturnType.Distance2Mul,
				CellularReturnType.Distance2Sub => FastNoiseLite.CellularReturnType.Distance2Sub,
				_ => throw new ArgumentException()
			};
		}

		private static FastNoiseLite.DomainWarpType ConvertDomainWarpType(DomainWarpType value) {
			return value switch {
				DomainWarpType.BasicGrid => FastNoiseLite.DomainWarpType.BasicGrid,
				DomainWarpType.OpenSimplex2 => FastNoiseLite.DomainWarpType.OpenSimplex2,
				DomainWarpType.OpenSimplex2Reduced => FastNoiseLite.DomainWarpType.OpenSimplex2Reduced,
				_ => throw new ArgumentException()
			};
		}

		private static FastNoiseLite.FractalType ConvertFractalType(FractalType value) {
			return value switch {
				FractalType.None => FastNoiseLite.FractalType.None,
				FractalType.DomainWarpIndependent => FastNoiseLite.FractalType.DomainWarpIndependent,
				FractalType.DomainWarpProgressive => FastNoiseLite.FractalType.DomainWarpProgressive,
				FractalType.FBm => FastNoiseLite.FractalType.FBm,
				FractalType.PingPong => FastNoiseLite.FractalType.PingPong,
				FractalType.Ridged => FastNoiseLite.FractalType.Ridged,
				_ => throw new ArgumentException()
			};
		}

		private static FastNoiseLite.NoiseType ConvertNoiseType(NoiseType value) {
			return value switch {
				NoiseType.OpenSimplex2 => FastNoiseLite.NoiseType.OpenSimplex2,
				NoiseType.OpenSimplex2S => FastNoiseLite.NoiseType.OpenSimplex2S,
				NoiseType.Cellular => FastNoiseLite.NoiseType.Cellular,
				NoiseType.Perlin => FastNoiseLite.NoiseType.Perlin,
				NoiseType.ValueCubic => FastNoiseLite.NoiseType.ValueCubic,
				NoiseType.Value => FastNoiseLite.NoiseType.Value,
				_ => throw new ArgumentException()
			};
		}

		private static FastNoiseLite.RotationType3D ConvertRotationType3D(RotationType3D value) {
			return value switch {
				RotationType3D.None => FastNoiseLite.RotationType3D.None,
				RotationType3D.ImproveXYPlanes => FastNoiseLite.RotationType3D.ImproveXYPlanes,
				RotationType3D.ImproveXZPlanes => FastNoiseLite.RotationType3D.ImproveXZPlanes,
				_ => throw new ArgumentException()
			};
		}
	}
}
