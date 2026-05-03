using SoulboundEngine.Client.Render.Item;

namespace SoulboundEngine.Client.UI.Screens {
	public sealed class WorldSessionScreenFactory : IScreenObjectFactory {
		private readonly IScreenObjectFactory originalFactory;
		private readonly ItemRenderManager itemRenderManager;
	
		public WorldSessionScreenFactory(ItemRenderManager itemRenderManager, IScreenObjectFactory originalFactory) {
			this.originalFactory = originalFactory;
			this.itemRenderManager = itemRenderManager;
		}

		public UnityEngine.GameObject CreateGameObject() {
			return this.originalFactory.CreateGameObject();
		}

		public IScreenObject CreateSceneObject(Screen screen, UnityEngine.GameObject obj) {
			WorldSessionScreenObject sessionObject = obj.AddComponent<WorldSessionScreenObject>();
			sessionObject.Init(this.itemRenderManager, screen);
			return sessionObject;
		}
	}
}
