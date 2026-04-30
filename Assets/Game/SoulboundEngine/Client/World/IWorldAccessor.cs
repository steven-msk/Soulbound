using System.Collections.Generic;

namespace SoulboundEngine.Client.World {
	public interface IWorldAccessor {
		void EnterWorld(string world);
		void QuitActiveWorld();
		IEnumerable<WorldSave> ListWorldSaves();
		bool IsWorldSessionActive();
		void CreateNewWorld(string world, int seed);
		void DeleteWorld(string world);
	}
}
