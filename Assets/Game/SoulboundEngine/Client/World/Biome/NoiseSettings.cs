using Vendor.FastNoiseLite;

namespace SoulboundEngine.Client.World.Generation {
	public record NoiseSettings {
		public FastNoiseLite.NoiseType noiseType = FastNoiseLite.NoiseType.OpenSimplex2;
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

		public void ApplyTo(FastNoiseLite noise) {
			noise.SetNoiseType(this.noiseType);
			noise.SetDomainWarpType(this.domainWarpType);
			noise.SetFractalType(this.fractalType);
			noise.SetRotationType3D(this.rotationType3D);
			noise.SetCellularDistanceFunction(this.cellularDistanceFunction);
			noise.SetCellularReturnType(this.cellularReturnType);
			noise.SetFrequency(this.frequency);
			noise.SetDomainWarpAmp(this.domainWarpAmp);
			noise.SetFractalGain(this.fractalGain);
			noise.SetFractalLacunarity(this.fractalLacunarity);
			noise.SetFractalOctaves(this.fractalOctaves);
			noise.SetFractalPingPongStrength(this.fractalPingPingStrength);
			noise.SetFractalWeightedStrength(this.fractalWeightedStrength);
			noise.SetCellularJitter(this.cellularJitter);
		}
	}
}
