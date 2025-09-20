using NUnit.Framework;
using SoulboundBackend.Client;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Core;
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
	[Test]
	public void Inventory_InitializesWithEmptySlots_OnSceneLoad() {
		SceneManager.SetActiveScene(SceneManager.CreateScene("testScene"));

		GameObject playerPrefab = ResourceManager.Get<GameObject, ResourceGroups.Runtime.Prefabs>("player");
		PlayerController player = GameObject.Instantiate(playerPrefab).GetComponent<PlayerController>();
		GameObject invPrefab = ResourceManager.Get<GameObject, ResourceGroups.Runtime.Prefabs>("Inventory");
		Assert.That(invPrefab != null, () => "Could not find Inventory prefab");
		InventoryController inventory = GameObject.Instantiate(invPrefab).GetComponent<InventoryController>();
		Assert.That(inventory != null, () => "Could not instantiate InventoryController");

		inventory.OnBootstrap(player);

		var f = inventory.GetFirstEmptySlot();
		Assert.IsTrue(inventory.GetFirstEmptySlot().ItemStack == null, $"{f.name} is not empty");
	}

	[Test]
	public void CreateItemDisplay_AssignsToFirstEmptySlot_WhenSlotExists() {
		SceneManager.SetActiveScene(SceneManager.CreateScene("testScene"));

		GameObject playerPrefab = ResourceManager.Get<GameObject, ResourceGroups.Runtime.Prefabs>("player");
		PlayerController player = GameObject.Instantiate(playerPrefab).GetComponent<PlayerController>();
		GameObject invPrefab = ResourceManager.Get<GameObject, ResourceGroups.Runtime.Prefabs>("Inventory");
		Assert.That(invPrefab != null, () => "Could not find Inventory prefab");
		InventoryController inventory = GameObject.Instantiate(invPrefab).GetComponent<InventoryController>();
		Assert.That(inventory != null, () => "Could not instantiate InventoryController");
		inventory.OnBootstrap(player);

		ItemStack stack = new ItemStack(Items.consumableStatItem_test, 1);
		var slot = inventory.GetFirstEmptySlot();
		Assert.IsNotNull(slot, "No empty slot available");
		ItemDisplay display = ItemDisplay.Create(stack, slot);
		Assert.That(slot.ItemDisplay, Is.EqualTo(display), () => "ItemDisplay did not assign correctly in slot");
	}
}
