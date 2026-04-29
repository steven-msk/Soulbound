using NSubstitute;
using NUnit.Framework;
using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.ItemSystem.Container;

namespace ItemTests.Container.Operations {
	internal class TransferTransitTests : SingleSlotOperationTests<TransferTransit> {
		protected override TransferTransit GetOperation(IItemContainer container, int slotIndex, IItemContainerScope scope) {
			return new TransferTransit(container, slotIndex, scope);
		}

		[Test]
		public void CanExecute_SlotHasStack_ReturnsTrue() {
			this.CreateOperation(null, this.fakeItem.CreateStack());

			Assert.That(this.operation.CanExecute(), Is.True);
		}

		[Test]
		public void CanExecute_ScopeHasTransitStack_ReturnsTrue() {
			this.CreateOperation(this.fakeItem.CreateStack(), null);

			Assert.That(this.operation.CanExecute(), Is.True);
		}


		[Test]
		public void CanExecute_BothHaveStacks_ReturnsTrue() {
			this.CreateOperation(this.fakeItem.CreateStack(), this.fakeItem.CreateStack());

			Assert.That(this.operation.CanExecute(), Is.True);
		}

		[Test]
		public void CanExecute_NeitherHasStack_ReturnsFalse() {
			this.CreateOperation(null, null);

			Assert.That(this.operation.CanExecute(), Is.False);
		}


		[Test]
		public void CanExecute_BothHaveStacks_DifferentItems_ReturnsTrue() {
			Item other = new FakeItem(DEFAULT_FULL_STACK);
			this.CreateOperation(other.CreateStack(), this.fakeItem.CreateStack());

			Assert.That(this.operation.CanExecute(), Is.True);
		}

		[Test]
		public void Execute_CannotExecute_ReturnsFalse() {
			this.CreateOperation(null, null);

			Assert.That(this.operation.Execute(), Is.False);
		}

		[Test]
		public void Execute_SlotHasStack_TransitHasStack_MergesAndReturnsTrue() {
			this.CreateOperation(this.fakeItem.CreateStack(5), this.fakeItem.CreateStack(10));

			Assert.That(this.operation.Execute(), Is.True);
			Assert.That(this.scope.HasTransitStack(), Is.False);
			Assert.That(this.slot.HasStack(), Is.True);
			Assert.That(this.slot.GetStack().quantity, Is.EqualTo(15));

			this.scope.Received().SetTransitStack(Arg.Is((ItemStack)null));
			this.slot.DidNotReceive().SetStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_SlotHasStack_NoTransitStack_GrabsAndReturnsTrue() {
			ItemStack slotStack = this.fakeItem.CreateStack(10);
			this.CreateOperation(null, slotStack);

			Assert.That(this.operation.Execute(), Is.True);
			Assert.That(this.scope.HasTransitStack(), Is.True);
			Assert.That(this.slot.HasStack(), Is.False);

			this.scope.Received().SetTransitStack(Arg.Is(slotStack));
			this.slot.Received().SetStack(Arg.Is((ItemStack)null));
		}

		[Test]
		public void Execute_SlotHasStack_TransitHasStack_DifferentItems_SwapsAndReturnsTrue() {
			Item other = new FakeItem(DEFAULT_FULL_STACK);
			ItemStack transitStack = other.CreateStack();
			ItemStack slotStack = this.fakeItem.CreateStack();
			this.CreateOperation(transitStack, slotStack);

			Assert.That(this.operation.Execute(), Is.True);
			Assert.That(this.slot.HasStack(), Is.True);
			Assert.That(this.scope.HasTransitStack(), Is.True);

			this.scope.Received().SetTransitStack(Arg.Is(slotStack));
			this.slot.Received().SetStack(Arg.Is(transitStack));
		}

		[Test]
		public void Execute_NoSubOperationApplies_ReturnsFalse() {
			this.CreateOperation(null, null);

			Assert.That(this.operation.Execute(), Is.False);
		}
	}
}
