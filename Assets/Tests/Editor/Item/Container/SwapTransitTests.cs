using NSubstitute;
using NUnit.Framework;
using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.ItemSystem.Container;

namespace ItemTests.Container.Operations {
	internal class SwapTransitTests : SingleSlotOperationTests<SwapTransit> {
		private FakeItem fakeItemA;
		private FakeItem fakeItemB;

		[SetUp]
		public new void Setup() {
			this.scope = Substitute.For<IItemContainerScope>();
			this.fakeItemA = new FakeItem(DEFAULT_FULL_STACK);
			this.fakeItemB = new FakeItem(DEFAULT_FULL_STACK);
		}

		protected override SwapTransit GetOperation(IItemContainer container, int slotIndex, IItemContainerScope scope) {
			return new SwapTransit(container, slotIndex, scope);
		}

		[Test]
		public void CanExecute_SlotHasStack_And_TransitHasStack_ReturnsTrue() {
			this.CreateOperation(this.fakeItemA.CreateStack(), this.fakeItemB.CreateStack());

			Assert.That(this.operation.CanExecute(), Is.True);
		}

		[Test]
		public void CanExecute_EmptySlot_ReturnsFalse() {
			this.CreateOperation(this.fakeItemA.CreateStack(), null);

			Assert.That(this.operation.CanExecute(), Is.False);
		}

		[Test]
		public void CanExecute_NoTransitStack_ReturnsFalse() {
			this.CreateOperation(null, this.fakeItemB.CreateStack());

			Assert.That(this.operation.CanExecute(), Is.False);
		}


		[Test]
		public void Execute_SwapsStacksCorrectly() {
			ItemStack slotStack = this.fakeItemB.CreateStack();
			ItemStack transitStack = this.fakeItemA.CreateStack();
			this.CreateOperation(transitStack, slotStack);

			Assert.That(this.operation.Execute(), Is.True);
			Assert.That(this.scope.GetTransitStack(), Is.EqualTo(slotStack));
			Assert.That(this.slot.GetStack(), Is.EqualTo(transitStack));

			this.scope.Received().SetTransitStack(Arg.Is<ItemStack>(slotStack));
			this.slot.Received().SetStack(Arg.Is<ItemStack>(transitStack));
		}

		[Test]
		public void Execute_WhenCannotExecute_ReturnsFalse() {
			this.CreateOperation(null, null);

			Assert.That(this.operation.CanExecute(), Is.False);
		}
	}
}
