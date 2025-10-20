using SoulboundBackend.Client;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.Structure.Templates;
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
	[SerializeField] private GameConfig gameConfig;
	private static GameEntryPoint instance;

    private void Awake() => instance = this;

    public static Func<BootstrapTreeBuilder, IEnumerable<IBootstrappable>> defaultBootstrapTree = treeBuilder => {
		return treeBuilder.BuildTree<BootstrappableParentOfAttribute>(typeof(LevelManager));
	};

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	public static void StartWorld() {
#if UNITY_INCLUDE_TESTS
		if (Application.isEditor && SceneManager.GetActiveScene().name != instance?.gameConfig.dev.devScene) {
			return;
		}
#endif
		Soulbound soulbound = new Soulbound(instance.gameConfig);
		soulbound.Prototype_LoadDevWorld();
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void ResetStaticDomain() {
		StaticResetManager.ResetAll();
		ResourceManager.PreloadGroups();
	}
}
