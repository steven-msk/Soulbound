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

public sealed class GameEntryPoint : MonoBehaviour, IStaticResettable {
    private LevelManager levelManager;
    private static BootstrappableInstanceFactory defaultInstanceFactory = null;

    public static BootstrappableInstanceFactory DefaultInstanceFactory(LevelManager levelManager) {
        if (defaultInstanceFactory != null) {
            return defaultInstanceFactory;
        }
        BootstrappableInstanceFactory factory = new();
        var playerInstancePrefab = ResourceManager.Get<GameObject, ResourceGroups.Runtime.Prefabs>("player");
        var uiManager = (GameObject.FindWithTag("MainCanvas") 
            ?? GameObject.Instantiate(ResourceManager.Get<GameObject, ResourceGroups.Runtime.Prefabs>("Canvas")))
            .GetComponent<UIManager>();

        var player = GameObject.Instantiate(playerInstancePrefab).GetComponent<PlayerController>();
        var inventory = uiManager.InstantiateInUILevel(player.Inventory).GetComponent<InventoryController>();

        factory.Register<LevelManager>(() => levelManager);
        factory.Register<PlayerController>(() => player);
        factory.Register<InventoryController>(() => inventory);
        factory.Register<HotbarController>(() => inventory.Hotbar);
        factory.Register<PlayerPhysics>(() => player.GetComponent<PlayerPhysics>());
        factory.Register<InputHandler>(() => GameObject.Instantiate(player.InputHandler).GetComponent<InputHandler>());
        factory.Register<ItemUsageHandler>(() => new ItemUsageHandler(player));

        defaultInstanceFactory = factory;
        return factory;
    }

    private void Awake() {
        WorldManager worldManager = new("saves");
        worldManager.LoadWorld("dev");
    }

    private void OnValidate() {
        StaticResetManager.Register(this);
        StaticResetManager.ResetAll();
        ResourceGroups.Bootstrap();
    }

    public void StaticReset() => defaultInstanceFactory = null;
}
