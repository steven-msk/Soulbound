using SoulboundBackend.Client;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Bootstrap;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public sealed class GameEntryPoint : MonoBehaviour {
    private LevelManager levelManager;

    private void Awake() {
#if !UNITY_EDITOR
			ResourceGroups.Bootstrap();
#else
        StaticResetManager.ResetAll();
#endif
        var playerInstancePrefab = ResourceManager.Get<GameObject, ResourceGroups.Runtime.Prefabs>("player");
        var uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        var player = GameObject.Instantiate(playerInstancePrefab).GetComponent<PlayerController>();
        var inventory = uiManager.InstantiateInUILevel(player.Inventory).GetComponent<InventoryController>();
        var hotbar = inventory.Hotbar;
        var inputHandler = GameObject.Instantiate(player.InputHandler).GetComponent<InputHandler>();
        var playerPhysics = player.GetComponent<PlayerPhysics>();
        var itemUsageHandler = new ItemUsageHandler(player);

        GameObject levelManagerPrefab = ResourceManager.Get<GameObject, ResourceGroups.Runtime.Prefabs>("levelManager");
        this.levelManager = GameObject.Instantiate(levelManagerPrefab).GetComponent<LevelManager>();
        List<IBootstrappable> bootstrappables = new() {
            player, inventory, hotbar, inputHandler, playerPhysics, itemUsageHandler, levelManager
        };
        levelManager.Init(bootstrappables, treeBuilder => treeBuilder.BuildTree<BootstrappableParentOfAttribute>(typeof(LevelManager)));
        levelManager.BootstrapWorld();
    }

    private void OnValidate() => ResourceGroups.Bootstrap();
}
