using SoulboundEngine.Client.UI.Screens;
using SoulboundEngine.Core;

namespace SoulboundEngine.Client.UI.Screens {
	public sealed class WorldSessionScreenFactory : IScreenObjectFactory {
		private readonly IScreenObjectFactory originalFactory;
	
		public WorldSessionScreenFactory(IScreenObjectFactory originalFactory) {
			this.originalFactory = originalFactory;
		}

		public UnityEngine.GameObject CreateGameObject() {
			return originalFactory.CreateGameObject();
		}

		public IScreenObject CreateSceneObject(Screen screen, UnityEngine.GameObject obj) {
			WorldSessionScreenObject sessionObject = obj.AddComponent<WorldSessionScreenObject>();
			sessionObject.Init(screen);
			return sessionObject;
		}
	}
}
