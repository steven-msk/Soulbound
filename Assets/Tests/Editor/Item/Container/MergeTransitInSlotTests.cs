using NSubstitute;
using NUnit.Framework;
using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.ItemSystem.Container;

namespace ItemTests.Container.Operations {
	internal class MergeTransitInSlotTests : SingleSlotOperationTests<MergeTransitInSlot> {
		protected override MergeTransitInSlot GetOperation(IItemContainer container, int slotIndex, IItemContainerScope scope) {
			return new MergeTransitInSlot(container, slotIndex, scope);
		}

		[Test]
		public void CanExecute_SlotEmpty_TransitStackExists_ReturnsTrue() {
			this.CreateOperation(this.fakeItem.CreateStack(), null);

			Assert.That(this.operation.CanExecute(), Is.True);
		}

		[Test]
		public void CanExecute_SlotEmpty_NoTransitStack_ReturnsFalse() {
			this.CreateOperation(null, null);

			Assert.That(this.operation.CanExecute(), Is.False);
		}

		[Test]
		public void CanExecute_SlotHasStackableStack_ReturnsTrue() {
			this.CreateOperation(this.fakeItem.CreateStack(), this.fakeItem.CreateStack());

			Assert.That(this.operation.CanExecute(), Is.True);
		}

		[Test]
		public void CanExecute_SlotHasNonStackableStack_ReturnsFalse() {
			FakeItem item = new(DEFAULT_FULL_STACK);
			this.CreateOperation(this.fakeItem.CreateStack(), item.CreateStack());

			Assert.That(this.operation.CanExecute(), Is.False);
		}

		[Test]
		public void CanExecute_SlotFull_StackableTransitStack_ReturnsTrue() {
			this.CreateOperation(this.fakeItem.CreateStack(), this.fakeItem.CreateStack(this.fakeItem.fullStackSize));

			Assert.That(this.operation.CanExecute(), Is.True);
		}

		[Test]
		public void Execute_SlotEmpty_DelegatesToReleaseTransitInEmptySlot() {
			ItemStack transitStack = this.fakeItem.CreateStack();
			this.CreateOperation(transitStack, null);

			Assert.That(this.operation.Execute(), Is.True);
			Assert.That(this.slot.HasStack(), Is.True);
			Assert.That(this.slot.GetStack(), Is.EqualTo(transitStack));
			Assert.That(this.scope.HasTransitStack(), Is.False);

			this.scope.Received().SetTransitStack(Arg.Is((ItemStack)null));
			this.slot.Received().SetStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_CannotExecute_ReturnsFalse() {
			this.CreateOperation(null, null);

			Assert.That(this.operation.Execute(), Is.False);

			this.scope.DidNotReceive().SetTransitStack(Arg.Any<ItemStack>());
			this.slot.DidNotReceive().SetStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_SlotFullNoSpace_ReturnsFalse() {
			this.CreateOperation(this.fakeItem.CreateStack(), this.fakeItem.CreateStack(this.fakeItem.fullStackSize));

			Assert.That(this.operation.Execute(), Is.False);

			this.scope.DidNotReceive().SetTransitStack(Arg.Any<ItemStack>());
			this.slot.DidNotReceive().SetStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_PartialTransfer_WhenSlotSpaceIsInsufficient() {
			this.fakeItem = new FakeItem(10);
			this.CreateOperation(this.fakeItem.CreateStack(5), this.fakeItem.CreateStack(8));

			Assert.That(this.operation.Execute(), Is.True);
			Assert.That(this.scope.HasTransitStack(), Is.True);
			Assert.That(this.scope.GetTransitStack().quantity, Is.EqualTo(3));
			Assert.That(this.slot.GetStack().IsFull(), Is.True);
		}

		[Test]
		public void Execute_FullSpace_TransfersFullTransitQuantity() {
			this.CreateOperation(this.fakeItem.CreateStack(10), null);

			Assert.That(this.operation.Execute(), Is.True);
			Assert.That(this.scope.HasTransitStack(), Is.False);
			Assert.That(this.slot.HasStack(), Is.True);
			Assert.That(this.slot.GetStack().quantity, Is.EqualTo(10));
		}

		[Test]
		public void Execute_TransfersCorrectAmount_IncrementsSlotStack_DecrementsTransitStack() {
			this.fakeItem = new FakeItem(10);
			ItemStack slotStack = this.fakeItem.CreateStack(7);
			ItemStack transitStack = this.fakeItem.CreateStack(5);
			this.CreateOperation(transitStack, slotStack);

			Assert.That(this.operation.Execute(), Is.True);
			Assert.That(slotStack.quantity, Is.EqualTo(10));
			Assert.That(transitStack.quantity, Is.EqualTo(2));

			this.scope.DidNotReceive().SetTransitStack(Arg.Any<ItemStack>());
			this.slot.DidNotReceive().SetStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_TransfersFullTransit_WhenTransitQuantityIsEqualToSpaceLeft() {
			this.fakeItem = new FakeItem(10);
			this.CreateOperation(this.fakeItem.CreateStack(4), this.fakeItem.CreateStack(6));

			Assert.That(this.operation.Execute(), Is.True);
			Assert.That(this.scope.HasTransitStack(), Is.False);
			Assert.That(this.slot.GetStack().IsFull(), Is.True);
		}

	}
}
