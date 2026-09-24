namespace SoulboundEngine.World.Serialization {
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Gen;

	public sealed record LevelPropertyInfo(int seed, WorldPreset preset) {
			public static readonly Codec<LevelPropertyInfo> CODEC = RecordCodec<LevelPropertyInfo, int, WorldPreset>.Of(
				Field.Required<LevelPropertyInfo, int>("seed", Codecs.INT, i => i.seed),
				Field.Required<LevelPropertyInfo, WorldPreset>("preset", WorldPreset.CODEC, i => i.preset),
				(seed, preset) => new LevelPropertyInfo(seed, preset)
			);
		}
}
