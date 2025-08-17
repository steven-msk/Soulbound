using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class NewTestScript {
	// A Test behaves as an ordinary method
	[Test]
	public void NewTestScriptSimplePasses() {
		// Use the Assert class to test conditions
	}

	// A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
	// `yield return null;` to skip a frame.
	[UnityTest]
	public IEnumerator NewTestScriptWithEnumeratorPasses() {
		SceneManager.LoadScene("SampleScene");
		yield return null;
		Assert.IsNotNull(GameManager.instance.Player?.Inventory, "Inventory never initialized");
		var inv = GameManager.instance.Player.Inventory;
		var f = inv.GetFirstEmptySlot();
		Assert.IsTrue(inv.GetFirstEmptySlot().ItemStack == null, $"{f.name} is not empty");
	}
}
