namespace SoulboundEngine.World.Serialization {
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Gen;

	public interface IWorldSaveValidator {
		bool IsValid(File saveFolder);
		void ValidateNewSave(File saveFolder, int seed, WorldPreset preset);

		WorldSave? Validate(File saveFolder);
	}
}
