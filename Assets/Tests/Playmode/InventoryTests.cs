using NUnit.Framework;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class InventoryTests {

	[UnityTest]
	public IEnumerator Inventory_InitializesEmptySlots_OnSceneLoad() {
		SceneManager.LoadScene("SampleScene");
		yield return null;
		Assert.IsNotNull(GameManager.instance.Player?.Inventory, "Inventory never initialized");
		var inv = GameManager.instance.Player.Inventory;
		var f = inv.GetFirstEmptySlot();
		Assert.IsTrue(inv.GetFirstEmptySlot().ItemStack == null, $"{f.name} is not empty");
	}

	[UnityTest]
	public IEnumerator CreateItemDisplay_AssignsToFirstEmptySlot_WhenSlotExists() {
		SceneManager.LoadScene("SampleScene");
		yield return null;
		var inv = GameManager.instance?.Player?.Inventory;
		Assert.IsNotNull(inv, "Inventory never initialized");
		ItemStack stack = new ItemStack(Items.consumableStatItem_test, 1);
		var slot = inv.GetFirstEmptySlot();
		Assert.IsNotNull(slot, "No empty slot available");
		ItemDisplay display = ItemDisplay.Create(stack, slot);
		Assert.That(slot.ItemDisplay, Is.EqualTo(display), () => "ItemDisplay did not assign correctly in slot");
	}
}
