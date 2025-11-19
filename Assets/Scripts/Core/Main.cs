using SoulboundBackend.Client;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.Settings;
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
using UnityEngine.UI;

public sealed class Main : MonoBehaviour {
	private LevelManager levelManager;
	[SerializeField] private GameConfig gameConfig;
	private static Main instance;
	private static Soulbound soulbound;

    private void Awake() => instance = this;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	public static void StartWorld() {
#if UNITY_INCLUDE_TESTS
		if (Application.isEditor && SceneManager.GetActiveScene().name != instance?.gameConfig.dev.devScene
				|| (!instance?.gameObject.activeSelf ?? true)) {
			return;
		}
#endif
		soulbound = new Soulbound(instance.gameConfig);
		var uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
		uiManager.SetScreen(new TitleScreen());
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void ResetStaticDomain() {
		ResourceManager.PreloadGroups();
	}

	private void OnApplicationQuit() {
		soulbound.OnApplicationQuit();
	}
}
