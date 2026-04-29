using NSubstitute;
using NUnit.Framework;
using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.ItemSystem.Container;

namespace ItemTests.Container.Operations {
	internal class TransferSingleToSlotTests : SingleSlotOperationTests<TransferSingleToSlot> {
		protected override TransferSingleToSlot GetOperation(IItemContainer container, int slotIndex, IItemContainerScope scope) {
			return new TransferSingleToSlot(container, slotIndex, scope);
		}

		[Test]
		public void CanExecute_SlotEmpty_NoTransitStack_ReturnsFalse() {
			this.CreateOperation(null, null);

			Assert.That(this.operation.CanExecute(), Is.False);
		}

		[Test]
		public void CanExecute_SlotEmpty_HasTransitStack_ReturnsTrue() {
			this.CreateOperation(this.fakeItem.CreateStack(), null);

			Assert.That(this.operation.CanExecute(), Is.True);
		}

		[Test]
		public void CanExecute_SlotOccupied_StackableWithTransit_ReturnsTrue() {
			this.CreateOperation(this.fakeItem.CreateStack(), this.fakeItem.CreateStack());

			Assert.That(this.operation.CanExecute(), Is.True);
		}

		[Test]
		public void CanExecute_SlotOccupied_NotStackableWithTransit_ReturnsFalse() {
			Item other = new FakeItem(DEFAULT_FULL_STACK);
			this.CreateOperation(other.CreateStack(), this.fakeItem.CreateStack());

			Assert.That(this.operation.CanExecute(), Is.False);
		}

		[Test]
		public void Execute_WhenCannotExecute_ReturnsFalseWithNoSideEffects() {
			this.CreateOperation(null, null);

			Assert.That(this.operation.Execute(), Is.False);
			Assert.That(this.slot.HasStack(), Is.False);
			Assert.That(this.scope.HasTransitStack(), Is.False);

			this.slot.DidNotReceive().SetStack(Arg.Any<ItemStack>());
			this.scope.DidNotReceive().SetTransitStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_SlotEmpty_ClonesOneItemIntoSlot() {
			this.CreateOperation(this.fakeItem.CreateStack(), null);

			Assert.That(this.operation.Execute(), Is.True);
			Assert.That(this.slot.HasStack(), Is.True);
			Assert.That(this.slot.GetStack().quantity, Is.EqualTo(1));

			this.slot.Received().SetStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_SlotEmpty_DecrementsTransitStack() {
			this.CreateOperation(this.fakeItem.CreateStack(5), null);

			Assert.That(this.operation.Execute(), Is.True);
			Assert.That(this.scope.HasTransitStack(), Is.True);
			Assert.That(this.scope.GetTransitStack().quantity, Is.EqualTo(4));

			this.scope.DidNotReceive().SetTransitStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_SlotEmpty_ReturnsTrue() {
			this.CreateOperation(this.fakeItem.CreateStack(), null);

			Assert.That(this.operation.Execute(), Is.True);
		}

		[Test]
		public void Execute_SlotOccupied_IncrementsSlotStack() {
			this.CreateOperation(this.fakeItem.CreateStack(), this.fakeItem.CreateStack(4));

			Assert.That(this.operation.Execute(), Is.True);
			Assert.That(this.slot.HasStack(), Is.True);
			Assert.That(this.slot.GetStack().quantity, Is.EqualTo(5));

			this.slot.DidNotReceive().SetStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_SlotOccupied_DecrementsTransit() {
			this.CreateOperation(this.fakeItem.CreateStack(6), this.fakeItem.CreateStack());

			Assert.That(this.operation.Execute(), Is.True);
			Assert.That(this.scope.HasTransitStack(), Is.True);
			Assert.That(this.scope.GetTransitStack().quantity, Is.EqualTo(5));

			this.scope.DidNotReceive().SetTransitStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_SlotOccupied_StackFull_DoesNotDecrementTransit() {
			this.CreateOperation(this.fakeItem.CreateStack(5), this.fakeItem.CreateStack(this.fakeItem.fullStackSize));

			Assert.That(this.operation.Execute(), Is.True);
			Assert.That(this.scope.HasTransitStack(), Is.True);
			Assert.That(this.scope.GetTransitStack().quantity, Is.EqualTo(5));

			this.scope.DidNotReceive().SetTransitStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_SlotOccupied_StackFull_ReturnsTrue() {
			this.CreateOperation(this.fakeItem.CreateStack(5), this.fakeItem.CreateStack(this.fakeItem.fullStackSize));

			Assert.That(this.operation.Execute(), Is.True);
		}


	}
}
