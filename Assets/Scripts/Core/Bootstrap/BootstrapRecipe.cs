using SoulboundBackend.Client;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace SoulboundBackend.Core.Bootstrap {
    public static class BootstrapRecipe {
        public static BootstrappableInstanceFactory ForPredefinedScene(LevelManager levelManager) {
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
            factory.Register<InputHandler>(() => GameObject.Instantiate(player.inputHandler));
            factory.Register<ItemUsageHandler>(() => new ItemUsageHandler(player));

            return factory;
        }

        public static BootstrappableInstanceFactory ForCompletelyNewScene(LevelManager levelManager) {
            BootstrappableInstanceFactory factory = new();

            factory.Register<LevelManager>(() => levelManager);

            return factory;
        }

        public static BootstrappableInstanceFactory ForInstanceCreation(out LevelManager levelManager) {
            levelManager = LevelManager.CreateInstance();

            T FindByType<T>() where T : UnityEngine.Object {
                return GameObject.FindAnyObjectByType<T>()
                    ?? throw new ArgumentException($"No object of type {typeof(T)} found in the current scene!");
            }

            var playerPrefab = ResourceManager.Get<GameObject, ResourceGroups.Runtime.Prefabs>("player");
            var player = GameObject.Instantiate(playerPrefab)!.GetComponent<PlayerController>();

            var instanceFactory = BootstrapRecipe.ForCompletelyNewScene(levelManager)
                .WithOverrides(factory => {
                    factory.Register<PlayerController>(() => player);
                    factory.Register<InventoryController>(FindByType<InventoryController>);
                    factory.Register<HotbarController>(FindByType<HotbarController>);
                    factory.Register<PlayerPhysics>(() => player.GetComponent<PlayerPhysics>());
                    factory.Register<InputHandler>(FindByType<InputHandler>);
                    factory.Register<ItemUsageHandler>(() => new ItemUsageHandler(player));
                });

            return instanceFactory;
        }

        public static BootstrappableInstanceFactory WithOverrides(
            this BootstrappableInstanceFactory factory,
            Action<BootstrappableInstanceFactory> overrides
        ) {
            overrides(factory);
            return factory;
        }
    }
}
