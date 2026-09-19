namespace SoulboundEngine.World.Gen {
	public readonly struct FeatureGenStep {
		public static readonly FeatureGenStep VEGETATION = new("vegetation");
		public static readonly FeatureGenStep[] INDEXED_VALUES = new[] {
			VEGETATION,
		};
		public readonly string serializedName;

		public FeatureGenStep(string serializedName) {
			this.serializedName = serializedName;
		}
	}
}
