using NUnit.Framework;
using SoulboundBackend.Client;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Bootstrap;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class InventoryTests {
	private InventoryController CreateTestEnvironment() {
        SceneManager.SetActiveScene(SceneManager.CreateScene("testScene"));

        GameObject playerPrefab = ResourceManager.Get<GameObject, ResourceGroups.Runtime.Prefabs>("player");
        PlayerController player = GameObject.Instantiate(playerPrefab).GetComponent<PlayerController>();
        InventoryController inventory = GameObject.Instantiate(player.Inventory).GetComponent<InventoryController>();
        Assert.That(inventory != null, () => "Could not instantiate InventoryController");

        Bootstrapper bootstrapper = new();
        BootstrapTreeBuilder treeBuilder = new(new List<IBootstrappable>() { player, inventory });
        var tree = treeBuilder.BuildTree<BootstrappableChildOfAttribute>(typeof(InventoryController));
        bootstrapper.Bootstrap(tree, bootstrapper.EarlyBootstrap(tree));

		return inventory;
    }

	[Test]
	public void Inventory_InitializesWithEmptySlots_OnNewSceneLoad() {
		var inventory = CreateTestEnvironment();

        var f = inventory.GetFirstEmptySlot();
		Assert.IsTrue(inventory.GetFirstEmptySlot().ItemStack == null, $"{f.name} is not empty");
	}

	[Test]
	public void CreateItemDisplay_AssignsToFirstEmptySlot_WhenSlotExists() {
		var inventory = CreateTestEnvironment();

		var slot = inventory.GetFirstEmptySlot();
		Assert.IsNotNull(slot, "No empty slot available");

		ItemStack stack = new ItemStack(Items.consumableStatItem_test, 1);
		ItemDisplay display = ItemDisplay.Create(stack, slot);
		Assert.That(slot.ItemDisplay, Is.EqualTo(display), () => "ItemDisplay did not assign correctly in slot");
	}
}
