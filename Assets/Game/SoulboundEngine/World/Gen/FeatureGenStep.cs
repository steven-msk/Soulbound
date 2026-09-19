namespace SoulboundEngine.World.Gen {
	using System.Collections.Generic;

#nullable enable

	public readonly struct FeatureGenStep {
		public static readonly FeatureGenStep VEGETATION = Register("vegetation", 0);
		private static readonly List<FeatureGenStep> INDEXED_VALUES = new();
		private static FeatureGenStep[]? ordinalSteps;
		public readonly int ordinalIndex;
		public readonly string serializedName;

		public FeatureGenStep(string serializedName, int ordinalIndex) {
			this.serializedName = serializedName;
			this.ordinalIndex = ordinalIndex;
		}

		private static FeatureGenStep Register(string serializedName, int ordinalIndex) {
			return Register(new FeatureGenStep(serializedName, ordinalIndex));
		}

		private static FeatureGenStep Register(FeatureGenStep step) {
			INDEXED_VALUES.Insert(step.ordinalIndex, step);
			return step;
		}

		public static FeatureGenStep[] GetOrdinal() => ordinalSteps ??= INDEXED_VALUES.ToArray();
	}
}
