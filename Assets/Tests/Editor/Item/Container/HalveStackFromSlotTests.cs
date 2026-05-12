using NSubstitute;
using NUnit.Framework;
using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.ItemSystem.Container;

namespace ItemTests.Container.Operations {
	internal class HalveStackFromSlotTests : SingleSlotOperationTests<HalveStackFromSlot> {
		protected override HalveStackFromSlot GetOperation(IItemContainer container, int slotIndex, IItemContainerScope scope) {
			return new HalveStackFromSlot(container, slotIndex, scope);
		}

		[Test]
		public void CanExecute_ReturnsFalse_WhenSlotIsEmpty() {
			this.CreateOperation(null, null);

			Assert.That(this.operation.CanExecute(), Is.False);
		}

		[Test]
		public void CanExecute_ReturnsFalse_WhenTransitStackExists() {
			this.CreateOperation(this.fakeItem.CreateStack(), null);

			Assert.That(this.operation.CanExecute(), Is.False);
		}

		[Test]
		public void CanExecute_ReturnsTrue_WhenSlotHasStackAndNoTransit() {
			this.CreateOperation(null, this.fakeItem.CreateStack());

			Assert.That(this.operation.CanExecute(), Is.True);
		}

		[Test]
		public void Execute_ReturnsFalse_WhenCannotExecute() {
			this.CreateOperation(null, null);

			Assert.That(this.operation.Execute(), Is.False);

			this.scope.DidNotReceive().SetTransitStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_ReturnsTrue_OnSuccess() {
			this.CreateOperation(null, this.fakeItem.CreateStack(4));

			Assert.That(this.operation.Execute(), Is.True);

			this.scope.Received().SetTransitStack(Arg.Any<ItemStack>());
			this.slot.DidNotReceive().SetStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_SetsSlotStackToNull_WhenQuantityIsOne() {
			this.CreateOperation(null, this.fakeItem.CreateStack());

			Assert.That(this.operation.Execute(), Is.True);

			this.scope.Received().SetTransitStack(Arg.Any<ItemStack>());
			this.slot.Received().SetStack(Arg.Is((ItemStack)null));
		}

		[Test]
		public void Execute_SetsTransitStack_WithCeilingHalf_WhenQuantityIsOdd() {
			this.CreateOperation(null, this.fakeItem.CreateStack(5));

			Assert.That(this.operation.Execute(), Is.True);
			Assert.That(this.scope.HasTransitStack(), Is.True);
			Assert.That(this.scope.GetTransitStack().quantity, Is.EqualTo(3));
		}

		[Test]
		public void Execute_SetsTransitStack_WithExactHalf_WhenQuantityIsEven() {
			this.CreateOperation(null, this.fakeItem.CreateStack(4));

			Assert.That(this.operation.Execute(), Is.True);
			Assert.That(this.scope.HasTransitStack(), Is.True);
			Assert.That(this.scope.GetTransitStack().quantity, Is.EqualTo(2));
		}

		[Test]
		public void Execute_DecrementsSlotStack_ByCeilingHalf_WhenQuantityIsOdd() {
			this.CreateOperation(null, this.fakeItem.CreateStack(5));

			Assert.That(this.operation.Execute(), Is.True);
			Assert.That(this.slot.HasStack(), Is.True);
			Assert.That(this.slot.GetStack().quantity, Is.EqualTo(2));
		}

		[Test]
		public void Execute_DecrementsSlotStack_ByExactHalf_WhenQuantityIsEven() {
			this.CreateOperation(null, this.fakeItem.CreateStack(4));

			Assert.That(this.operation.Execute(), Is.True);
			Assert.That(this.slot.HasStack(), Is.True);
			Assert.That(this.slot.GetStack().quantity, Is.EqualTo(2));
		}

		[Test]
		public void Execute_ClearsSlotStack_WhenQuantityIsOne() {
			this.CreateOperation(null, this.fakeItem.CreateStack());

			Assert.That(this.operation.Execute(), Is.True);
			Assert.That(this.slot.HasStack(), Is.False);
		}

		[Test]
		public void Execute_ClearsSlotStack_WhenFullStackSizeIsOne() {
			this.fakeItem = new FakeItem(1);
			this.CreateOperation(null, this.fakeItem.CreateStack());

			Assert.That(this.operation.Execute(), Is.True);
			Assert.That(this.slot.HasStack(), Is.False);
		}

		[Test]
		public void Execute_SetsTransitStack_WithFullStack_WhenQuantityIsOne() {
			this.fakeItem = new FakeItem(1);
			this.CreateOperation(null, this.fakeItem.CreateStack());

			Assert.That(this.operation.Execute(), Is.True);
			Assert.That(this.scope.HasTransitStack(), Is.True);
			Assert.That(this.scope.GetTransitStack().IsFull(), Is.True);
		}

		[Test]
		public void Execute_TransitStack_IsIndependentClone_OfSlotStack() {
			this.CreateOperation(null, this.fakeItem.CreateStack(10));

			Assert.That(this.operation.Execute(), Is.True);
			Assert.That(this.scope.HasTransitStack(), Is.True);

			this.scope.GetTransitStack().Increment(1);
			Assert.That(this.slot.GetStack().quantity, Is.EqualTo(5));
			Assert.That(this.scope.GetTransitStack().quantity, Is.EqualTo(6));

			this.scope.Received().SetTransitStack(Arg.Any<ItemStack>());
			this.slot.DidNotReceive().SetStack(Arg.Any<ItemStack>());
		}
	}
}
