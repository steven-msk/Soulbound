using SoulboundBackend.Client;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Client.World;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Bootstrap;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class GameEntryPoint : MonoBehaviour {
	private LevelManager levelManager;

	public static Func<LevelManager, BootstrappableInstanceFactory> DefaultInstanceFactory() {
		return levelManager => {
			BootstrappableInstanceFactory factory = new();
			var playerInstancePrefab = ResourceManager.Get<GameObject, ResourceGroups.Runtime.Prefabs>("player");
			var uiManager = (GameObject.FindWithTag("MainCanvas")
				?? GameObject.Instantiate(ResourceManager.Get<GameObject, ResourceGroups.Runtime.Prefabs>("Canvas")))
				.GetComponent<UIManager>();

			var player = GameObject.Instantiate(playerInstancePrefab).GetComponent<PlayerController>();
			player.enabled = false;
			var inventory = uiManager.InstantiateInUILevel(player.Inventory).GetComponent<InventoryController>();

			factory.Register<LevelManager>(() => levelManager);
			factory.Register<PlayerController>(() => player);
			factory.Register<InventoryController>(() => inventory);
			factory.Register<HotbarController>(() => inventory.Hotbar);
			factory.Register<PlayerPhysics>(() => player.GetComponent<PlayerPhysics>());
			factory.Register<InputHandler>(() => GameObject.Instantiate(player.InputHandler));
			factory.Register<ItemUsageHandler>(() => new ItemUsageHandler(player));

			return factory;
		};
	}

	public static Func<BootstrapTreeBuilder, IEnumerable<IBootstrappable>> defaultBootstrapTree = treeBuilder => {
		return treeBuilder.BuildTree<BootstrappableParentOfAttribute>(typeof(LevelManager));
    };


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	public static void StartWorld() {
#if UNITY_INCLUDE_TESTS
		if (Application.isEditor && SceneManager.GetActiveScene().name != "DevScene") {
			return;
		}
#endif
		WorldManager worldManager = new("saves", new WorldSaveStrategy());
		var loadedDump = worldManager.LoadWorld("devWorld", null, defaultBootstrapTree, true);
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void ResetStaticDomain() {
        StaticResetManager.ResetAll();
        ResourceGroups.Bootstrap();
    }
}
