using System.Collections.Generic;

namespace SoulboundEngine.Client.World {
	public interface IWorldAccessor {
		void EnterWorld(string world);
		void QuitActiveWorld();
		IEnumerable<string> ListWorldSaves();
		bool IsWorldSessionActive();
		void CreateNewWorld(string world);
		void DeleteWorld(string world);
	}
}
