namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Common.Math.Random;
	using System;

	public sealed class NormalNoise {
		private const double INPUT_FACTOR = 1.0181268882175227d;
		private readonly int firstOctave;
		private readonly double[] octaveMultipliers;
		private readonly OctavePerlinNoise first;
		private readonly OctavePerlinNoise second;
		private readonly double valueFactor;
		private readonly Parameters parameters;

		private NormalNoise(IRandom random, Parameters parameters) {
			this.octaveMultipliers = parameters.octaveMultipliers;
			this.firstOctave = parameters.firstOctave;
			this.parameters = parameters;

			this.first = new OctavePerlinNoise(random, this.firstOctave, this.octaveMultipliers);
			this.second = new OctavePerlinNoise(random, this.firstOctave, this.octaveMultipliers);
			int minOctave = int.MaxValue;
			int maxOctave = int.MinValue;
			for (int i = 0; i < this.octaveMultipliers.Length; i++) {
				if (this.octaveMultipliers[i] != 0.0d) {
					minOctave = Math.Min(minOctave, i);
					maxOctave = Math.Max(maxOctave, i);
				}
			}
			this.valueFactor = 0.16666666666666666 / ExpectedDeviation(maxOctave - minOctave);
		}

		public static NormalNoise Create(IRandom random, int firstOctave, params double[] octaveMultipliers) {
			return new NormalNoise(random, new Parameters(firstOctave, octaveMultipliers));
		}

		public static NormalNoise Create(IRandom random, Parameters parameters) {
			return new NormalNoise(random, parameters);
		}

		public double Get(double x, double y, double z) {
			double x2 = x * INPUT_FACTOR;
			double y2 = y * INPUT_FACTOR;
			double z2 = z * INPUT_FACTOR;
			return (this.first.Get(x, y, z) + this.second.Get(x2, y2, z2)) * this.valueFactor;
		}

		private static double ExpectedDeviation(int octaveSpan) {
			return 0.1 * (1.0 + 1.0 / (octaveSpan + 1));
		}

		public Parameters GetParameters() => this.parameters;

		public sealed record Parameters(int firstOctave, double[] octaveMultipliers) {
			public static Parameters Of(int firstOctave, params double[] octaveMultipliers) {
				return new Parameters(firstOctave, octaveMultipliers);
			}
		}
	}


}
