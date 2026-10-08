namespace SoulboundEngine.World {
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Gen;
	using SoulboundEngine.World.Serialization;
	using System.Collections.Generic;

	public interface IWorldAccessor {
		void EnterWorld(string world);
		void QuitActiveWorld();
		IEnumerable<WorldSave> ListWorldSaves();
		bool IsWorldSessionActive();
		void CreateNewWorld(string world, int seed, RegistryEntry<WorldPreset> preset);
		void DeleteWorld(string world);
	}
}
