namespace SoulboundEngine.World {
	using SoulboundEngine.World.Serialization;
	using System.Collections.Generic;

	public interface IWorldAccessor {
		void EnterWorld(string world);
		void QuitActiveWorld();
		IEnumerable<WorldSave> ListWorldSaves();
		bool IsWorldSessionActive();
		void CreateNewWorld(string world, int seed);
		void DeleteWorld(string world);
	}
}
