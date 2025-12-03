using NUnit.Framework;
using SoulboundBackend.Client;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Common;
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
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Zenject;

namespace InventoryTests {
	[TestFixture]
	public abstract class InventoryTest {
		internal InventoryController CreateTestEnvironment(out Scene scene) {
			scene = PlayModeTesting.CreateNewSceneAndSetActive();
			var prefab = ResourceManager.GetRuntimePrefab("sceneContext");
			var canvas = GameObject.Instantiate(ResourceManager.GetRuntimePrefab("Canvas")).GetComponent<Canvas>();
			var sceneContext = GameObject.Instantiate(prefab).GetComponent<SceneContext>();
			var player = GameObject.Instantiate(ResourceManager.GetRuntimePrefab("player")).GetComponent<PlayerController>();

			var actionMap = new PlayerInputActions().asset.FindActionMap("Player");
			sceneContext.AddNormalInstaller(new PlayerInstaller(player, canvas, new InputHandler(actionMap)));
			sceneContext.Install();
			return sceneContext.Container.Resolve<InventoryController>();
		}

		internal Item CreateTestItem(int maxStack = 64) {
			return new GenericItem("testItem", new ItemAspect(new ItemIcon(null, 100), () => null), maxStack);
		}

		internal ItemDisplay SetGrabbedContext(ItemStack stack, InventoryController inventory) {
			ItemDisplay grabbedDisplay = ItemDisplay.Create(stack, () => null);
			inventory.GrabbedContext.Set(grabbedDisplay, null);
			return grabbedDisplay;
		}

		internal IItemSlot PlaceStackInFirstEmptySlot(ItemStack itemStack, InventoryController inventory) {
			IItemSlot slot = inventory.GetFirstEmptySlot();
			slot.CreateDisplay(itemStack);
			return slot;
		}
	}

	[TestFixture]
	public class Initialization : InventoryTest {
		[Test]
		public void Inventory_InitializesWithEmptySlots_OnNewSceneLoad() {
			var inventory = CreateTestEnvironment(out var scene);

			var f = inventory.GetFirstEmptySlot();
			Assert.IsTrue(inventory.GetFirstEmptySlot().stack == null, $"{f.name} is not empty");
		}

		[Test]
		public void CreateItemDisplay_AssignsToFirstEmptySlot_WhenSlotExists() {
			var inventory = CreateTestEnvironment(out var scene);

			var slot = inventory.GetFirstEmptySlot() as IItemSlot;
			Assert.IsNotNull(slot, "No empty slot available");

			ItemStack stack = new ItemStack(Items.consumableStatItem_test, 1);
			ItemDisplay display = slot.CreateDisplay(stack);
			Assert.That(slot.itemDisplay, Is.EqualTo(display),
				() => "ItemDisplay did not assign correctly in slot");
		}
	}

	[TestFixture]
	public class Serialization : InventoryTest {
		[Test]
		public void Serialize_SavesEmptyInventory_WhenNoItemsPresent() {
			var inventory = CreateTestEnvironment(out var scene);

			var serialized = inventory.Serialize();
			Assert.That(serialized.regions.Values.All(list => list.All(slot => slot.itemStack == null)));
		}

		[Test]
		public void Deserialize_ThrowsInvalidRegion_WhenRegionDoesNotExist() {
			var inventory = CreateTestEnvironment(out var scene);
			var serialized = new SerializedInventory(0, new Dictionary<string, List<SerializedItemSlot>>());

			LogAssert.Expect(LogType.Error, "[InventoryController]: Inventory region not found: hotbar");
			inventory.Deserialize(serialized);
		}

		[UnityTest]
		public IEnumerator Serialize_And_Deserialize_PreservesItemStacks() {
			Item item = CreateTestItem();
			var inventory = CreateTestEnvironment(out var scene);
			var slot = inventory.GetFirstEmptySlot() as IItemSlot;

			ItemStack stack = new ItemStack(item, 64);
			LogAssert.ignoreFailingMessages = true;
			slot.CreateDisplay(stack);

			var serialized = inventory.Serialize();
			yield return PlayModeTesting.UnloadSceneAsync(scene);

			inventory = CreateTestEnvironment(out scene);
			inventory.Deserialize(serialized);

			var occupiedSlot = inventory.GetOccupiedSlots(item).First();
			Assert.That(occupiedSlot.stack, Is.EqualTo(stack));
			Assert.That(occupiedSlot.index, Is.EqualTo(slot.index));
		}
	}
}

namespace InventoryTests.Logic {
	[TestFixture]
	public class PickupItem : InventoryTest {
		[Test]
		public void PickupItem_AddsItemToInventory_WhenSpaceAvailable() {
			var inventory = CreateTestEnvironment(out var scene);
			Item item = CreateTestItem();
			ItemStack stack = new ItemStack(item, 10);

			var pickedUp = inventory.PickUpItem(stack, out int remaining);

			Assert.That(pickedUp, Is.True);
			Assert.That(remaining, Is.EqualTo(0));
			Assert.That(inventory.GetOccupiedSlots(item).First().stack.quantity == stack.quantity);
		}

		[Test]
		public void PickupItem_DoesNotAddItemToInventory_WhenNoSpaceAvailable() {
			var inventory = CreateTestEnvironment(out var scene);
			Item item = CreateTestItem();
			ItemStack stack = new ItemStack(item, 10);

			foreach (var slot in inventory.MainPlayerSlots.Cast<IItemSlot>()) {
				slot.CreateDisplay(new ItemStack(item, item.maxStackSize));
			}

			var pickedUp = inventory.PickUpItem(stack, out int remaining);
			Assert.That(pickedUp, Is.False);
			Assert.That(remaining, Is.EqualTo(stack.quantity));
		}

		[Test]
		public void PickupItem_AddsPartialStackToFirstEmptySlot_WhenFirstStackFillsUp() {
			var inventory = CreateTestEnvironment(out var scene);
			Item item = CreateTestItem();

			ItemStack fullStack = new(item, item.maxStackSize);
			ItemStack patialStack = new(item, item.maxStackSize - 10);

			void PickUp(ItemStack stack) {
				bool pickedUp = inventory.PickUpItem(stack, out int remaining);
				Assert.That(pickedUp, Is.True);
				Assert.That(remaining, Is.EqualTo(0));
			}
			PickUp(patialStack);
			PickUp(fullStack);

			var occupiedSlots = inventory.GetOccupiedSlots(item).ToList();
			Assert.That(occupiedSlots[0].stack.quantity, Is.EqualTo(fullStack.quantity));
			Assert.That(occupiedSlots[1].stack.quantity, Is.EqualTo(patialStack.quantity));
		}

		[Test]
		public void PickupItem_FillsNextEmptySlot_ForNonStackableItem() {
			var inventory = CreateTestEnvironment(out var scene);
			var item = CreateTestItem(maxStack: 1);
			var singleStack = new ItemStack(item, 1);

			(inventory.GetFirstEmptySlot() as IItemSlot).CreateDisplay(singleStack);

			bool pickedUp = inventory.PickUpItem(singleStack, out int remaining);
			Assert.That(pickedUp, Is.True);
			Assert.That(remaining, Is.EqualTo(0));

			var occupiedSlots = inventory.GetOccupiedSlots(item).ToList();
			Assert.That(occupiedSlots.Count, Is.EqualTo(2));
			Assert.That(occupiedSlots.All(slot => slot.stack.IsFull() && slot.stack.quantity == 1));
		}
	}

	[TestFixture]
	public class MergeInStack : InventoryTest {
		[Test]
		public void MergeInSlot_PartiallyMerges_WhenSlotStackIsNotFull() {
			var inventory = CreateTestEnvironment(out var scene);
			var item = CreateTestItem();

			var fullStack = new ItemStack(item, 64);
			var partialStack = new ItemStack(item, 50);
			var grabbedStack = new ItemStack(item, 30);

			IItemSlot slot1 = inventory[0, 0];
			IItemSlot slot2 = inventory[0, 1];
			slot1.CreateDisplay(fullStack);
			slot2.CreateDisplay(partialStack);

			var grabbedReference = inventory.CreateGrabbedReference(grabbedStack);
			bool merged = inventory.MergeInSlot(slot2, grabbedReference);

			Assert.That(merged, Is.False);
			Assert.That(slot1.stack.quantity, Is.EqualTo(64));
			Assert.That(slot2.stack.quantity, Is.EqualTo(item.maxStackSize));
			Assert.That(grabbedReference.value, Is.Not.Null);
			Assert.That(grabbedReference.value.stack.quantity, Is.EqualTo(16));
		}

		[Test]
		public void MergeInSlot_FullyMerges_WhenSlotStackIsEmpty() {
			var inventory = CreateTestEnvironment(out var scene);
			var item = CreateTestItem();
			var grabbedStack = new ItemStack(item, 30);

			RefBox<ItemDisplay> grabbedReference = inventory.CreateGrabbedReference(grabbedStack);
			bool merged = inventory.MergeInSlot(inventory.GetFirstEmptySlot(), grabbedReference);

			Assert.That(merged, Is.True);
			Assert.That(grabbedReference.value, Is.Null);
			Assert.That(inventory.GetOccupiedSlots(item).First().stack.quantity, Is.EqualTo(30));
		}

		[Test]
		public void MergeInSlot_FillsUpSlotStack_WhenGrabbedStackFits() {
			var inventory = CreateTestEnvironment(out var scene);
			var item = CreateTestItem();
			var grabbedStack = new ItemStack(item, 10);
			var slotStack = new ItemStack(item, item.maxStackSize - 10);

			var slot = PlaceStackInFirstEmptySlot(slotStack, inventory);
			RefBox<ItemDisplay> grabbedReference = inventory.CreateGrabbedReference(grabbedStack);
			bool merged = inventory.MergeInSlot(slot, grabbedReference);

			Assert.That(merged, Is.True);
			Assert.That(grabbedReference.value, Is.Null);
			Assert.That(slot.stack.quantity, Is.EqualTo(item.maxStackSize));
		}

		[Test]
		public void MergeInSlot_DoesNotMerge_WhenNoSpaceAvailable() {
			var inventory = CreateTestEnvironment(out var scene);
			var item = CreateTestItem();
			var grabbedStack = new ItemStack(item, 10);
			var slotStack = new ItemStack(item, item.maxStackSize);

			var slot = PlaceStackInFirstEmptySlot(slotStack, inventory);
			RefBox<ItemDisplay> grabbedReference = inventory.CreateGrabbedReference(grabbedStack);
			bool merged = inventory.MergeInSlot(slot, grabbedReference);

			Assert.That(merged, Is.False);
			Assert.That(grabbedReference.value, Is.Not.Null);
			Assert.That(grabbedReference.value.stack.quantity, Is.EqualTo(10));
		}

		[Test]
		public void MergeInSlot_DoesNotMerge_WithDifferentItems() {
			var inventory = CreateTestEnvironment(out var scene);
			var item1 = CreateTestItem(maxStack: 1);
			var item2 = CreateTestItem(maxStack: 32);

			var grabbedStack = new ItemStack(item1, 1);
			var slotStack = new ItemStack(item2, 16);
			var slot = PlaceStackInFirstEmptySlot(slotStack, inventory);

			RefBox<ItemDisplay> grabbedReference = inventory.CreateGrabbedReference(grabbedStack);
			bool merged = inventory.MergeInSlot(slot, grabbedReference);

			Assert.That(merged, Is.False);
			Assert.That(grabbedReference.value, Is.Not.Null);
			Assert.That(grabbedReference.value.stack == grabbedStack);
		}
	}
}

namespace InventoryTests.Logic.Interpretation {
	[TestFixture]
	public class ClickInterpretations : InventoryTest {
		[Test]
		public void InterpretClick_ReturnsTransferGrabbed_WhenSlotOccupiedAndGrabbedEmpty_OnLeftClick() {
			var inventory = CreateTestEnvironment(out var scene);
			var item = CreateTestItem();
			var slotStack = new ItemStack(item, 10);

			var slot = PlaceStackInFirstEmptySlot(slotStack, inventory);
			inventory.GrabbedContext.Set(null, null);

			var func = inventory.InterpretClick(slot, PointerEventData.InputButton.Left, false, out bool cancelDrag);
			Assert.That(func == inventory.TransferGrabbed);
		}

		[Test]
		public void InterpretClick_ReturnsTransferGrabbed_WhenSlotEmptyAndGrabbedOccupied_OnLeftClick() {
			var inventory = CreateTestEnvironment(out var scene);
			var item = CreateTestItem();
			var grabbedStack = new ItemStack(item, 10);

			SetGrabbedContext(grabbedStack, inventory);

			IItemSlot slot = inventory.GetFirstEmptySlot();
			var func = inventory.InterpretClick(slot, PointerEventData.InputButton.Left, false, out bool cancelDrag);
			Assert.That(func == inventory.TransferGrabbed);
		}

		[Test]
		public void InterpretClick_ReturnsTransferGrabbed_WhenSlotOccupiedAndGrabbedOccupied_WithDifferentItems_OnLeftClick() {
			var inventory = CreateTestEnvironment(out var scene);
			var item1 = CreateTestItem();
			var item2 = CreateTestItem(maxStack: 1);
			var grabbedStack = new ItemStack(item1, 10);
			var slotStack = new ItemStack(item2, 1);

			SetGrabbedContext(grabbedStack, inventory);
			var slot = PlaceStackInFirstEmptySlot(slotStack, inventory);

			var func = inventory.InterpretClick(slot, PointerEventData.InputButton.Left, false, out bool cancelDrag);
			Assert.That(func == inventory.TransferGrabbed);
		}

		[Test]
		public void InterpretClick_ReturnsDoNothing_WhenRightClick_OnSlotWithItemDifferentToGrabbed() {
			var inventory = CreateTestEnvironment(out var scene);
			var item1 = CreateTestItem();
			var item2 = CreateTestItem(maxStack: 1);
			var grabbedStack = new ItemStack(item1, item1.maxStackSize);
			var slotStack = new ItemStack(item2, 1);

			SetGrabbedContext(grabbedStack, inventory);
			var slot = PlaceStackInFirstEmptySlot(slotStack, inventory);

			var func = inventory.InterpretClick(slot, PointerEventData.InputButton.Right, false, out bool cancelDrag);
			Assert.That(func == inventory.DoNothing);
		}

		[Test]
		public void InterpretClick_ReturnsTransferSingleToSlot_WhenRightClick_OnSlotEmptyAndGrabbedOccupied() {
			var inventory = CreateTestEnvironment(out var scene);
			var item = CreateTestItem();
			var grabbedStack = new ItemStack(item, 10);

			SetGrabbedContext(grabbedStack, inventory);
			IItemSlot slot = inventory.GetFirstEmptySlot();

			var func = inventory.InterpretClick(slot, PointerEventData.InputButton.Right, false, out bool cancelDrag);
			Assert.That(func == inventory.TransferSingleToSlot);
		}

		[Test]
		public void InterpretClick_ReturnsHalveStackFromSlot_WhenRightClick_OnSlotOccupiedAndGrabbedEmpty() {
			var inventory = CreateTestEnvironment(out var scene);
			var item = CreateTestItem();
			var slotStack = new ItemStack(item, 10);

			inventory.GrabbedContext.Set(null, null);
			var slot = PlaceStackInFirstEmptySlot(slotStack, inventory);

			var func = inventory.InterpretClick(slot, PointerEventData.InputButton.Right, false, out bool cancelDrag);
			Assert.That(func == inventory.HalveStackFromSlot);
		}

		[Test]
		public void InterpretClick_ReturnsCollectAllStacksInGrabbed_WhenDoubleLeftClick_And_GrabbedNotEmpty() {
			var inventory = CreateTestEnvironment(out var scene);
			var item = CreateTestItem();
			var grabbedStack = new ItemStack(item, 10);

			SetGrabbedContext(grabbedStack, inventory);

			IItemSlot slot = inventory.GetFirstEmptySlot();
			var func = inventory.InterpretClick(slot, PointerEventData.InputButton.Left, true, out bool cancelDrag);
			Assert.That(func == inventory.CollectAllStacksInGrabbed_Impl);
		}
	}

	[TestFixture]
	public class DragInterpretations : InventoryTest {
		[Test]
		public void InterpretDrag_ReturnsSplitDistributeToSlot_WhenDragLeftClick_AndGrabbedOccupied() {
			var inventory = CreateTestEnvironment(out var scene);
			var item = CreateTestItem();
			var grabbedStack = new ItemStack(item, 10);

			RefBox<ItemDisplay> grabbedReference = inventory.CreateGrabbedReference(grabbedStack);
			SetGrabbedContext(grabbedStack, inventory);
			IItemSlot slot = inventory.GetFirstEmptySlot();

			var dragHandler = inventory.StartDrag(slot, PointerEventData.InputButton.Left);
			var func = inventory.InterpretDrag(dragHandler, slot, grabbedReference);
			Assert.That(func == inventory.SplitDistributeToDraggedSlot_Impl);
		}

		[Test]
		public void InterpretDrag_ReturnsTransferSingleToSlot_WhenDragRightClick_AndGrabbedOccupied() {
			var inventory = CreateTestEnvironment(out var scene);
			var item = CreateTestItem();
			var grabbedStack = new ItemStack(item, 10);

			RefBox<ItemDisplay> grabbedReference = inventory.CreateGrabbedReference(grabbedStack);
			SetGrabbedContext(grabbedStack, inventory);
			IItemSlot slot = inventory.GetFirstEmptySlot();

			var dragHandler = inventory.StartDrag(slot, PointerEventData.InputButton.Right);
			var func = inventory.InterpretDrag(dragHandler, slot, grabbedReference);
			Assert.That(func == inventory.TransferSingleToSlot);
		}

		[Test]
		public void InterpretDrag_ReturnsDoNothing_WhenDragRightClick_AndGrabbedStackIsDifferentToSlot() {
			var inventory = CreateTestEnvironment(out var scene);
			var item1 = CreateTestItem();
			var item2 = CreateTestItem(maxStack: 32);
			var grabbedStack = new ItemStack(item1, 10);
			var slotStack = new ItemStack(item2, 1);

			RefBox<ItemDisplay> grabbedReference = inventory.CreateGrabbedReference(grabbedStack);
			SetGrabbedContext(grabbedStack, inventory);
			var slot = PlaceStackInFirstEmptySlot(slotStack, inventory);

			var dragHandler = inventory.StartDrag(slot, PointerEventData.InputButton.Right);
			var func = inventory.InterpretDrag(dragHandler, slot, grabbedReference);
			Assert.That(func == inventory.DoNothing);
		}
	}
}
