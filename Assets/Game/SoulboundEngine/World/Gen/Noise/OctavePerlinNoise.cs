namespace SoulboundEngine.World.Gen.Noise {
	using SoulboundEngine.Common.Math.Noise;
	using SoulboundEngine.Common.Math.Random;
	using System;

#nullable enable

	public sealed class OctavePerlinNoise {
		private readonly NoiseSampler?[] noiseLevels;
		private readonly int firstOctave;
		private readonly double[] octaveMultipliers;
		private readonly double lacunarity;
		private readonly double persistence;
		private readonly double lowestFreqValueFactor;
		private readonly double lowestFreqInputFactor;

		public OctavePerlinNoise(IRandom random, string salt, int firstOctave, double[] octaveMultipliers, double lacunarity, double persistence) {
			this.firstOctave = firstOctave;
			this.octaveMultipliers = octaveMultipliers;
			this.lacunarity = lacunarity;
			this.persistence = persistence;
			int octaves = octaveMultipliers.Length;
			this.noiseLevels = new NoiseSampler[octaves];
			IPositionalRandomFactory positionalFactory = random.ForkPositional();

			for (int i = 0; i < this.noiseLevels.Length; i++) {
				if (octaveMultipliers[i] != 0.0d) {
					int seed = positionalFactory.FromHashOf(salt + "_octave_" + i).NextInt();
					NoiseSettings settings = new(seed, NoiseType.Perlin);
					NoiseSampler sampler = new(i, settings);
					this.noiseLevels[i] = sampler;
				}
			}
			this.lowestFreqInputFactor = Math.Pow(2.0d, firstOctave);
			this.lowestFreqValueFactor = Math.Pow(2.0d, octaves - 1) / (Math.Pow(2.0d, octaves) - 1.0d);
		}

		public double Get(double x, double y, double z) {
			double value = 0.0d;
			double factor = this.lowestFreqInputFactor;
			double valueFactor = this.lowestFreqValueFactor;

			for (int i = 0; i < this.noiseLevels.Length; i++) {
				NoiseSampler? sampler = this.noiseLevels[i];
				if (sampler != null) {
					double noise = sampler.Sample3D((float)Wrap(x * factor), (float)Wrap(y * factor), (float)Wrap(z * factor));
					value += this.octaveMultipliers[i] * noise * valueFactor;
				}

				factor *= this.lacunarity;
				valueFactor *= this.persistence;
			}

			return value;
		}

		public static double Wrap(double x) {
			return x - (long)Math.Floor(x / 3.3554432E7d + 0.5) * 3.3554432E7d;
		}
	}
}
