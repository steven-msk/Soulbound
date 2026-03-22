using NSubstitute;
using NUnit.Framework;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.ItemSystem.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Graphs;


#nullable enable

namespace ItemTests.Container.Operations {
	internal class SplitDistributeToDraggedSlotTests : SlotOperationTest {
		private void CreateOperation(IItemSlot slot) {
			int index = slot.GetIndex();
			IItemContainer container = slot.GetContainer();
			operation = new SplitDistributeToDraggedSlot(new SlotRef(container, index), scope);
		}

		private void BuildDragState(ItemStack stack, IItemSlot origin, IEnumerable<IItemSlot> draggedSlots) {
			SlotRef originRef = new(origin.GetContainer(), origin.GetIndex());
			HashSet<SlotRef> dragged = new(new SlotRef.EqualityComparer()) { originRef };
			foreach (var slot in draggedSlots) dragged.Add(slot.GetRef());

			SlotDragState state = new(origin.GetContainer()) {
				origin = originRef,
				stack = stack.Clone(),
				button = UnityEngine.EventSystems.PointerEventData.InputButton.Left,
				draggedSlots = dragged,
				quantitySnapshots = CaptureQuantitySnapshots()
			};
			scope.InDragState().Returns(true);
			scope.GetDragState().Returns(state);
			scope.When(s => s.ExtendDrag(Arg.Any<SlotRef>()))
				.Do(callInfo => state.ExtendDrag(callInfo.Arg<SlotRef>()));
		}

		private Dictionary<SlotRef, int> CaptureQuantitySnapshots() {
			Dictionary<SlotRef, int> quantitySnapshots = new();

			foreach (var container in scope.GetOpenContainers()) {
				foreach (var index in container.GetAllSlots()) {
					IItemSlot slot = container.GetSlot(index);
					int quantity = slot.GetStack()?.quantity ?? 0;
					quantitySnapshots[new SlotRef(container, index)] = quantity;
				}
			}

			return quantitySnapshots;
		}

		[Test]
		public void CanExecute_WhenNotInDragState_ReturnsFalse() {
			IItemContainer container = ContainerUtils.SubstituteContainer((ItemStack?)null);
			IItemSlot slot = container.GetSlot(0);
			scope.InDragState().Returns(false);

			CreateOperation(slot);
			Assert.That(operation.CanExecute(), Is.False);
		}

		[Test]
		public void CanExecute_WhenSlotIsAlreadyDragged_ReturnsFalse() {
			IItemContainer container = ContainerUtils.NewEmptyContainer(scope);
			IItemSlot origin = container.AddSlot(fakeItem.CreateStack());
			IItemSlot targetSlot = container.AddSlot();
			IItemSlot[] dragged = new[] { targetSlot };
			BuildDragState(fakeItem.CreateStack(), origin, dragged);

			CreateOperation(targetSlot);
			Assert.That(operation.CanExecute(), Is.False);
		}

		[Test]
		public void CanExecute_WhenTargetSlotIsEmpty_ReturnsTrue() {
			IItemContainer container = ContainerUtils.NewEmptyContainer(scope);
			IItemSlot origin = container.AddSlot();
			IItemSlot targetSlot = container.AddSlot();
			BuildDragState(fakeItem.CreateStack(), origin, Array.Empty<IItemSlot>());
			
			CreateOperation(targetSlot);
			Assert.That(operation.CanExecute(), Is.True);
		}

		[Test]
		public void CanExecute_WhenTargetSlotHasSameItemAndIsNotFull_ReturnsTrue() {
			IItemContainer container = ContainerUtils.NewEmptyContainer(scope);
			IItemSlot origin = container.AddSlot();
			IItemSlot targetSlot = container.AddSlot(fakeItem.CreateStack());
			BuildDragState(fakeItem.CreateStack(), origin, Array.Empty<IItemSlot>());

			CreateOperation(targetSlot);
			Assert.That(operation.CanExecute(), Is.True);
		}

		[Test]
		public void CanExecute_WhenTargetSlotHasDifferentItem_ReturnsFalse() {
			Item otherItem = new FakeItem() {
				_fullStackSize = DEFAULT_FULL_STACK
			};
			IItemContainer container = ContainerUtils.NewEmptyContainer(scope);
			IItemSlot origin = container.AddSlot();
			IItemSlot targetSlot = container.AddSlot(otherItem.CreateStack());
			BuildDragState(fakeItem.CreateStack(), origin, Array.Empty<IItemSlot>());

			CreateOperation(targetSlot);
			Assert.That(operation.CanExecute(), Is.False);
		}

		[Test]
		public void CanExecute_WhenTargetSlotHasSameItemButIsFull_ReturnsFalse() {
			IItemContainer container = ContainerUtils.NewEmptyContainer(scope);
			IItemSlot origin = container.AddSlot();
			IItemSlot targetSlot = container.AddSlot(fakeItem.CreateStack(fakeItem.fullStackSize));
			BuildDragState(fakeItem.CreateStack(), origin, Array.Empty<IItemSlot>());

			CreateOperation(targetSlot);
			Assert.That(operation.CanExecute(), Is.False);
		}

		[Test]
		public void Execute_WhenCanExecuteIsFalse_ReturnsFalse() {
			IItemContainer container = ContainerUtils.SubstituteContainer((ItemStack?)null);
			IItemSlot slot = container.GetSlot(0);
			scope.InDragState().Returns(false);

			CreateOperation(slot);
			Assert.That(operation.Execute(), Is.False);
		}

		[Test]
		public void Execute_WhenSplitAmountIsZero_ReturnsFalse() {
			// 1 item split across origin + 1 new slot = 0 per slot (1 / 2)
			ItemStack stack = fakeItem.CreateStack();

			IItemContainer container = ContainerUtils.NewEmptyContainer(scope);
			IItemSlot origin = container.AddSlot();
			IItemSlot targetSlot = container.AddSlot();
			BuildDragState(stack, origin, Array.Empty<IItemSlot>());

			CreateOperation(targetSlot);
			Assert.That(operation.Execute(), Is.False);
		}

		[Test]
		public void Execute_EvenSplit_DistributesEquallyAcrossAllSlots() {
			IItemContainer container = ContainerUtils.NewEmptyContainer(scope);
			IItemSlot origin = container.AddSlot();
			IItemSlot targetSlot = container.AddSlot();
			IItemSlot dragged = container.AddSlot();
			BuildDragState(fakeItem.CreateStack(6), origin, new IItemSlot[] { dragged });

			CreateOperation(targetSlot);
			Assert.That(operation.Execute(), Is.True);

			Assert.That(origin.GetStack()?.quantity, Is.EqualTo(2));
			Assert.That(targetSlot.GetStack()?.quantity, Is.EqualTo(2));
			Assert.That(dragged.GetStack()?.quantity, Is.EqualTo(2));
		}

		[Test]
		public void Execute_EvenSplit_DoesNotCallSetStack_WhenTargetAlreadyHasStack() {
			IItemContainer container = ContainerUtils.NewEmptyContainer(scope);
			IItemSlot origin = container.AddSlot();
			IItemSlot targetSlot = container.AddSlot(fakeItem.CreateStack());
			BuildDragState(fakeItem.CreateStack(6), origin, Array.Empty<IItemSlot>());

			CreateOperation(targetSlot);
			Assert.That(operation.Execute(), Is.True);

			targetSlot.DidNotReceive().SetStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_WhenTargetSlotIsEmpty_CallsSetStack() {
			IItemContainer container = ContainerUtils.NewEmptyContainer(scope);
			IItemSlot origin = container.AddSlot();
			IItemSlot targetSlot = container.AddSlot();
			BuildDragState(fakeItem.CreateStack(10), origin, Array.Empty<IItemSlot>());

			CreateOperation(targetSlot);
			Assert.That(operation.Execute(), Is.True);

			targetSlot.Received().SetStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_UnevenSplit_FirstSlotReceivesExtraOne([Values(3, 5, 7, 9, 11)] int unevenQuantity) {
			IItemContainer container = ContainerUtils.NewEmptyContainer(scope);
			IItemSlot origin = container.AddSlot();
			IItemSlot targetSlot = container.AddSlot();
			BuildDragState(fakeItem.CreateStack(unevenQuantity), origin, Array.Empty<IItemSlot>());

			CreateOperation(targetSlot);
			Assert.That(operation.Execute(), Is.True);

			int originQuantity = origin.GetStack()?.quantity ?? 0;
			int targetQuantity = targetSlot.GetStack()?.quantity ?? 0;
			Assert.That(originQuantity + targetQuantity, Is.EqualTo(unevenQuantity));
			Assert.That(Math.Abs(originQuantity - targetQuantity), Is.LessThanOrEqualTo(1));
		}

		[Test]
		public void Execute_UnevenSplit_TotalQuantityAlwaysEqualsOriginStack([Values(3, 5, 7, 11, 13)] int quantity) {
			IItemContainer container = ContainerUtils.NewEmptyContainer(scope);
			IItemSlot origin = container.AddSlot();
			IItemSlot targetSlot = container.AddSlot();
			BuildDragState(fakeItem.CreateStack(quantity), origin, Array.Empty<IItemSlot>());

			CreateOperation(targetSlot);
			Assert.That(operation.Execute(), Is.True);

			int distributed = (origin.GetStack()?.quantity ?? 0) + (targetSlot.GetStack()?.quantity ?? 0);
			Assert.That(distributed, Is.EqualTo(quantity));
		}

		[Test]
		public void Execute_WhenSnapshotExistsForSlot_AddsSnapshotAmount() {
			IItemContainer container = ContainerUtils.NewEmptyContainer(scope);
			IItemSlot origin = container.AddSlot();
			IItemSlot targetSlot = container.AddSlot(fakeItem.CreateStack(5));
			BuildDragState(fakeItem.CreateStack(4), origin, Array.Empty<IItemSlot>());

			CreateOperation(targetSlot);
			Assert.That(operation.Execute(), Is.True);

			Assert.That(targetSlot.GetStack()?.quantity, Is.EqualTo(7));
		}

		[Test]
		public void Execute_WhenNoSnapshotExistsForSlot_UsesAmountOnly() {
			IItemContainer container = ContainerUtils.NewEmptyContainer(scope);
			IItemSlot origin = container.AddSlot();
			BuildDragState(fakeItem.CreateStack(4), origin, Array.Empty<IItemSlot>());

			// target slot created after the state was built -> no quantity snapshot for this slot
			// for this to be possible the slot needs to be empty
			IItemSlot targetSlot = container.AddSlot();

			CreateOperation(targetSlot);
			Assert.That(operation.Execute(), Is.True);

			Assert.That(targetSlot.GetStack()?.quantity, Is.EqualTo(2));
			targetSlot.Received().SetStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_CallsExtendDrag_BeforeDistributing() {
			IItemContainer container = ContainerUtils.NewEmptyContainer(scope);
			IItemSlot origin = container.AddSlot();
			IItemSlot targetSlot = container.AddSlot();
			BuildDragState(fakeItem.CreateStack(4), origin, Array.Empty<IItemSlot>());

			bool extendedBeforeSetQuantity = false;
			bool extendDragCalled = false;
			targetSlot.stackChanged += (_, _) => {
				extendedBeforeSetQuantity = extendDragCalled;
			};
			scope.When(s => s.ExtendDrag(Arg.Any<SlotRef>()))
				.Do(_ => extendDragCalled = true);

			CreateOperation(targetSlot);
			Assert.That(operation.Execute(), Is.True);
			Assert.That(extendedBeforeSetQuantity, Is.True);
		}

		[Test]
		public void Execute_CallsExtendDragExactlyOnce() {
			IItemContainer container = ContainerUtils.NewEmptyContainer(scope);
			IItemSlot origin = container.AddSlot();
			IItemSlot targetSlot = container.AddSlot();
			BuildDragState(fakeItem.CreateStack(4), origin, Array.Empty<IItemSlot>());

			CreateOperation(targetSlot);
			Assert.That(operation.Execute(), Is.True);

			scope.Received(1).ExtendDrag(Arg.Any<SlotRef>());
		}
	}
}
