namespace SoulboundEngine.Core.Noise {
	public record NoiseSettings {
		public NoiseType noiseType = NoiseType.OpenSimplex2;
		public DomainWarpType domainWarpType = DomainWarpType.OpenSimplex2;
		public FractalType fractalType = FractalType.None;
		public RotationType3D rotationType3D = RotationType3D.None;
		public CellularDistanceFunction cellularDistanceFunction = CellularDistanceFunction.Euclidean;
		public CellularReturnType cellularReturnType = CellularReturnType.CellValue;
		public float frequency = 0.01f;
		public float domainWarpAmp = 1.0f;
		public float fractalGain = 0.5f;
		public float fractalLacunarity = 2.0f;
		public int fractalOctaves = 3;
		public float fractalPingPingStrength = 2.0f;
		public float fractalWeightedStrength = 0.0f;
		public float cellularJitter = 0.0f;
		public int seed = 1337;

		public NoiseSettings() { }

		public NoiseSettings(NoiseType noiseType, float frequency) {
			this.noiseType = noiseType;
			this.frequency = frequency;
		}

		public NoiseSettings(int seed, NoiseType noiseType, float frequency = 0.01f) {
			this.noiseType = noiseType;
			this.seed = seed;
			this.frequency = frequency;
		}

		public void ApplyTo(FastNoiseLiteAdapter noise) {
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
			noise.SetSeed(this.seed);
		}
	}
}
