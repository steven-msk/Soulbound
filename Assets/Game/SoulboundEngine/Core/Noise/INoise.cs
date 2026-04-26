namespace SoulboundEngine.Core.Noise {
	public interface INoise : IDomainWarp {
		float GetNoise(float x, float y);
		float GetNoise(float x, float y, float z);

		void SetCellularDistanceFunction(CellularDistanceFunction cellularDistanceFunction);
		void SetCellularJitter(float cellularJitter);
		void SetCellularReturnType(CellularReturnType cellularReturnType);

		void SetFractalGain(float gain);
		void SetFractalLacunarity(float lacunarity);
		void SetFractalOctaves(int octaves);
		void SetFractalPingPongStrength(float pingPongStrength);
		void SetFractalType(FractalType fractalType);
		void SetFractalWeightedStrength(float weightedStrength);

		void SetFrequency(float frequency);
		void SetNoiseType(NoiseType noiseType);
		void SetSeed(int seed);

		void SetRotationType3D(RotationType3D rotationType3D);
	}
}
