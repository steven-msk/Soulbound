using System.Collections;
using NUnit.Framework;
using SoulboundBackend.Core;
using UnityEngine;
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
}
