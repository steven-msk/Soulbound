using SoulboundEngine.Core.Serialization;

namespace SoulboundEngine.Client.World.Serialization {
	public interface IWorldSaveValidator {
		bool IsValid(File saveFolder);
		void ValidateNewSave(File saveFolder, int seed);

		bool Validate(File saveFolder, out int seed, out string worldName, out File chunksFolder);
	}
}
