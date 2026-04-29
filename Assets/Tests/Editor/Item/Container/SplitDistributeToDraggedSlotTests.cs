using NSubstitute;
using NUnit.Framework;
using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.ItemSystem.Container;
using System;
using System.Collections.Generic;


#nullable enable

namespace ItemTests.Container.Operations {
	internal class SplitDistributeToDraggedSlotTests : SlotOperationTest {
		private void CreateOperation(IItemSlot slot) {
			int index = slot.GetIndex();
			IItemContainer container = slot.GetContainer();
			this.operation = new SplitDistributeToDraggedSlot(new SlotRef(container, index), this.scope);
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
				quantitySnapshots = this.CaptureQuantitySnapshots()
			};
			this.scope.InDragState().Returns(true);
			this.scope.GetDragState().Returns(state);
			this.scope.When(s => s.ExtendDrag(Arg.Any<SlotRef>()))
				.Do(callInfo => state.ExtendDrag(callInfo.Arg<SlotRef>()));
		}

		private Dictionary<SlotRef, int> CaptureQuantitySnapshots() {
			Dictionary<SlotRef, int> quantitySnapshots = new();

			foreach (var container in this.scope.GetOpenContainers()) {
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
			this.scope.InDragState().Returns(false);

			this.CreateOperation(slot);
			Assert.That(this.operation.CanExecute(), Is.False);
		}

		[Test]
		public void CanExecute_WhenSlotIsAlreadyDragged_ReturnsFalse() {
			IItemContainer container = ContainerUtils.NewEmptyContainer(this.scope);
			IItemSlot origin = container.AddSlot(this.fakeItem.CreateStack());
			IItemSlot targetSlot = container.AddSlot();
			IItemSlot[] dragged = new[] { targetSlot };
			this.BuildDragState(this.fakeItem.CreateStack(), origin, dragged);

			this.CreateOperation(targetSlot);
			Assert.That(this.operation.CanExecute(), Is.False);
		}

		[Test]
		public void CanExecute_WhenTargetSlotIsEmpty_ReturnsTrue() {
			IItemContainer container = ContainerUtils.NewEmptyContainer(this.scope);
			IItemSlot origin = container.AddSlot();
			IItemSlot targetSlot = container.AddSlot();
			this.BuildDragState(this.fakeItem.CreateStack(), origin, Array.Empty<IItemSlot>());

			this.CreateOperation(targetSlot);
			Assert.That(this.operation.CanExecute(), Is.True);
		}

		[Test]
		public void CanExecute_WhenTargetSlotHasSameItemAndIsNotFull_ReturnsTrue() {
			IItemContainer container = ContainerUtils.NewEmptyContainer(this.scope);
			IItemSlot origin = container.AddSlot();
			IItemSlot targetSlot = container.AddSlot(this.fakeItem.CreateStack());
			this.BuildDragState(this.fakeItem.CreateStack(), origin, Array.Empty<IItemSlot>());

			this.CreateOperation(targetSlot);
			Assert.That(this.operation.CanExecute(), Is.True);
		}

		[Test]
		public void CanExecute_WhenTargetSlotHasDifferentItem_ReturnsFalse() {
			Item otherItem = new FakeItem(DEFAULT_FULL_STACK);
			IItemContainer container = ContainerUtils.NewEmptyContainer(this.scope);
			IItemSlot origin = container.AddSlot();
			IItemSlot targetSlot = container.AddSlot(otherItem.CreateStack());
			this.BuildDragState(this.fakeItem.CreateStack(), origin, Array.Empty<IItemSlot>());

			this.CreateOperation(targetSlot);
			Assert.That(this.operation.CanExecute(), Is.False);
		}

		[Test]
		public void CanExecute_WhenTargetSlotHasSameItemButIsFull_ReturnsFalse() {
			IItemContainer container = ContainerUtils.NewEmptyContainer(this.scope);
			IItemSlot origin = container.AddSlot();
			IItemSlot targetSlot = container.AddSlot(this.fakeItem.CreateStack(this.fakeItem.fullStackSize));
			this.BuildDragState(this.fakeItem.CreateStack(), origin, Array.Empty<IItemSlot>());

			this.CreateOperation(targetSlot);
			Assert.That(this.operation.CanExecute(), Is.False);
		}

		[Test]
		public void Execute_WhenCanExecuteIsFalse_ReturnsFalse() {
			IItemContainer container = ContainerUtils.SubstituteContainer((ItemStack?)null);
			IItemSlot slot = container.GetSlot(0);
			this.scope.InDragState().Returns(false);

			this.CreateOperation(slot);
			Assert.That(this.operation.Execute(), Is.False);
		}

		[Test]
		public void Execute_WhenSplitAmountIsZero_ReturnsFalse() {
			// 1 item split across origin + 1 new slot = 0 per slot (1 / 2)
			ItemStack stack = this.fakeItem.CreateStack();

			IItemContainer container = ContainerUtils.NewEmptyContainer(this.scope);
			IItemSlot origin = container.AddSlot();
			IItemSlot targetSlot = container.AddSlot();
			this.BuildDragState(stack, origin, Array.Empty<IItemSlot>());

			this.CreateOperation(targetSlot);
			Assert.That(this.operation.Execute(), Is.False);
		}

		[Test]
		public void Execute_EvenSplit_DistributesEquallyAcrossAllSlots() {
			IItemContainer container = ContainerUtils.NewEmptyContainer(this.scope);
			IItemSlot origin = container.AddSlot();
			IItemSlot targetSlot = container.AddSlot();
			IItemSlot dragged = container.AddSlot();
			this.BuildDragState(this.fakeItem.CreateStack(6), origin, new IItemSlot[] { dragged });

			this.CreateOperation(targetSlot);
			Assert.That(this.operation.Execute(), Is.True);

			Assert.That(origin.GetStack()?.quantity, Is.EqualTo(2));
			Assert.That(targetSlot.GetStack()?.quantity, Is.EqualTo(2));
			Assert.That(dragged.GetStack()?.quantity, Is.EqualTo(2));
		}

		[Test]
		public void Execute_EvenSplit_DoesNotCallSetStack_WhenTargetAlreadyHasStack() {
			IItemContainer container = ContainerUtils.NewEmptyContainer(this.scope);
			IItemSlot origin = container.AddSlot();
			IItemSlot targetSlot = container.AddSlot(this.fakeItem.CreateStack());
			this.BuildDragState(this.fakeItem.CreateStack(6), origin, Array.Empty<IItemSlot>());

			this.CreateOperation(targetSlot);
			Assert.That(this.operation.Execute(), Is.True);

			targetSlot.DidNotReceive().SetStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_WhenTargetSlotIsEmpty_CallsSetStack() {
			IItemContainer container = ContainerUtils.NewEmptyContainer(this.scope);
			IItemSlot origin = container.AddSlot();
			IItemSlot targetSlot = container.AddSlot();
			this.BuildDragState(this.fakeItem.CreateStack(10), origin, Array.Empty<IItemSlot>());

			this.CreateOperation(targetSlot);
			Assert.That(this.operation.Execute(), Is.True);

			targetSlot.Received().SetStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_UnevenSplit_FirstSlotReceivesExtraOne([Values(3, 5, 7, 9, 11)] int unevenQuantity) {
			IItemContainer container = ContainerUtils.NewEmptyContainer(this.scope);
			IItemSlot origin = container.AddSlot();
			IItemSlot targetSlot = container.AddSlot();
			this.BuildDragState(this.fakeItem.CreateStack(unevenQuantity), origin, Array.Empty<IItemSlot>());

			this.CreateOperation(targetSlot);
			Assert.That(this.operation.Execute(), Is.True);

			int originQuantity = origin.GetStack()?.quantity ?? 0;
			int targetQuantity = targetSlot.GetStack()?.quantity ?? 0;
			Assert.That(originQuantity + targetQuantity, Is.EqualTo(unevenQuantity));
			Assert.That(Math.Abs(originQuantity - targetQuantity), Is.LessThanOrEqualTo(1));
		}

		[Test]
		public void Execute_UnevenSplit_TotalQuantityAlwaysEqualsOriginStack([Values(3, 5, 7, 11, 13)] int quantity) {
			IItemContainer container = ContainerUtils.NewEmptyContainer(this.scope);
			IItemSlot origin = container.AddSlot();
			IItemSlot targetSlot = container.AddSlot();
			this.BuildDragState(this.fakeItem.CreateStack(quantity), origin, Array.Empty<IItemSlot>());

			this.CreateOperation(targetSlot);
			Assert.That(this.operation.Execute(), Is.True);

			int distributed = (origin.GetStack()?.quantity ?? 0) + (targetSlot.GetStack()?.quantity ?? 0);
			Assert.That(distributed, Is.EqualTo(quantity));
		}

		[Test]
		public void Execute_WhenSnapshotExistsForSlot_AddsSnapshotAmount() {
			IItemContainer container = ContainerUtils.NewEmptyContainer(this.scope);
			IItemSlot origin = container.AddSlot();
			IItemSlot targetSlot = container.AddSlot(this.fakeItem.CreateStack(5));
			this.BuildDragState(this.fakeItem.CreateStack(4), origin, Array.Empty<IItemSlot>());

			this.CreateOperation(targetSlot);
			Assert.That(this.operation.Execute(), Is.True);

			Assert.That(targetSlot.GetStack()?.quantity, Is.EqualTo(7));
		}

		[Test]
		public void Execute_WhenNoSnapshotExistsForSlot_UsesAmountOnly() {
			IItemContainer container = ContainerUtils.NewEmptyContainer(this.scope);
			IItemSlot origin = container.AddSlot();
			this.BuildDragState(this.fakeItem.CreateStack(4), origin, Array.Empty<IItemSlot>());

			// target slot created after the state was built -> no quantity snapshot for this slot
			// for this to be possible the slot needs to be empty
			IItemSlot targetSlot = container.AddSlot();

			this.CreateOperation(targetSlot);
			Assert.That(this.operation.Execute(), Is.True);

			Assert.That(targetSlot.GetStack()?.quantity, Is.EqualTo(2));
			targetSlot.Received().SetStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_CallsExtendDrag_BeforeDistributing() {
			IItemContainer container = ContainerUtils.NewEmptyContainer(this.scope);
			IItemSlot origin = container.AddSlot();
			IItemSlot targetSlot = container.AddSlot();
			this.BuildDragState(this.fakeItem.CreateStack(4), origin, Array.Empty<IItemSlot>());

			bool extendedBeforeSetQuantity = false;
			bool extendDragCalled = false;
			targetSlot.stackChanged += (_, _) => {
				extendedBeforeSetQuantity = extendDragCalled;
			};
			this.scope.When(s => s.ExtendDrag(Arg.Any<SlotRef>()))
				.Do(_ => extendDragCalled = true);

			this.CreateOperation(targetSlot);
			Assert.That(this.operation.Execute(), Is.True);
			Assert.That(extendedBeforeSetQuantity, Is.True);
		}

		[Test]
		public void Execute_CallsExtendDragExactlyOnce() {
			IItemContainer container = ContainerUtils.NewEmptyContainer(this.scope);
			IItemSlot origin = container.AddSlot();
			IItemSlot targetSlot = container.AddSlot();
			this.BuildDragState(this.fakeItem.CreateStack(4), origin, Array.Empty<IItemSlot>());

			this.CreateOperation(targetSlot);
			Assert.That(this.operation.Execute(), Is.True);

			this.scope.Received(1).ExtendDrag(Arg.Any<SlotRef>());
		}
	}
}
