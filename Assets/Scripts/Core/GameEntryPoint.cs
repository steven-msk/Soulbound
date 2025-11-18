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

public sealed class GameEntryPoint : MonoBehaviour {
	private LevelManager levelManager;
	[SerializeField] private GameConfig gameConfig;
	private static GameEntryPoint instance;

    private void Awake() => instance = this;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	public static void StartWorld() {
#if UNITY_INCLUDE_TESTS
		if (Application.isEditor && SceneManager.GetActiveScene().name != instance?.gameConfig.dev.devScene
				|| (!instance?.gameObject.activeSelf ?? true)) {
			return;
		}
#endif
		Soulbound soulbound = new Soulbound(instance.gameConfig);
		var childContainer = GameObject.Find("Canvas").GetComponent<ChildReferenceContainer>();
		var enterWorldButton = childContainer.GetChildComponent<Button>("WorldEnter");
		enterWorldButton.onClick.AddListener(soulbound.Prototype_LoadDevWorld);
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void ResetStaticDomain() {
		StaticResetManager.ResetAll();
		ResourceManager.PreloadGroups();
	}
}
