using NUnit.Framework;
using SoulboundBackend.Client;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Bootstrap;
using SoulboundBackend.Core.Resource;
using SoulboundBackend.Tests;
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
        SceneManager.SetActiveScene(SceneManager.CreateScene(Guid.NewGuid().ToString()));

        GameObject levelManagerPrefab = ResourceManager.Get<GameObject, ResourceGroups.Runtime.Prefabs>("levelManager");
        var levelManager = GameObject.Instantiate(levelManagerPrefab).GetComponent<LevelManager>();

        levelManager.Init(null, null,
			BootstrapRecipe.ForPredefinedScene(levelManager),
			treeBuilder => treeBuilder.BuildTree<BootstrappableChildOfAttribute>(typeof(InventoryController))
		);

        return levelManager.Player.Inventory
			?? throw new ArgumentException("Test environment not initialized properly");
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
		Assert.That(slot.ItemDisplay, Is.EqualTo(display),
			() => "ItemDisplay did not assign correctly in slot");
	}

    [SetUp]
    public void PrepareEnvironment() {
        StaticResetManager.ResetAll();
    }
}
