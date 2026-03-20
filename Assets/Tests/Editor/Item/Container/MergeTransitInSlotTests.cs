using NSubstitute;
using NUnit.Framework;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.ItemSystem.Container;

namespace ItemTests.Container.Operations {
	public class MergeTransitInSlotTests : SingleSlotOperationTests<MergeTransitInSlot> {
		private const int DEFAULT_FULL_STACK = 256;
		private FakeItem fakeItem;

		[SetUp]
		public void Setup() {
			scope = Substitute.For<IItemContainerScope>();
			fakeItem = new FakeItem() {
				_fullStackSize = DEFAULT_FULL_STACK,
			};
		}

		protected override MergeTransitInSlot GetOperation(IItemContainer container, int slotIndex, IItemContainerScope scope) {
			return new MergeTransitInSlot(container, slotIndex, scope);
		}

		[Test]
		public void CanExecute_SlotEmpty_TransitStackExists_ReturnsTrue() {
			CreateOperation(fakeItem.CreateStack(), null);

			Assert.That(operation.CanExecute(), Is.True);
		}

		[Test]
		public void CanExecute_SlotEmpty_NoTransitStack_ReturnsFalse() {
			CreateOperation(null, null);

			Assert.That(operation.CanExecute(), Is.False);
		}

		[Test]
		public void CanExecute_SlotHasStackableStack_ReturnsTrue() {
			CreateOperation(fakeItem.CreateStack(), fakeItem.CreateStack());

			Assert.That(operation.CanExecute(), Is.True);
		}

		[Test]
		public void CanExecute_SlotHasNonStackableStack_ReturnsFalse() {
			FakeItem item = new();
			CreateOperation(fakeItem.CreateStack(), item.CreateStack());

			Assert.That(operation.CanExecute(), Is.False);
		}

		[Test]
		public void CanExecute_SlotFull_StackableTransitStack_ReturnsFalse() {
			CreateOperation(fakeItem.CreateStack(), fakeItem.CreateStack(fakeItem.fullStackSize));

			Assert.That(operation.CanExecute(), Is.False);
		}

		[Test]
		public void Execute_SlotEmpty_DelegatesToReleaseTransitInEmptySlot() {
			ItemStack transitStack = fakeItem.CreateStack();
			CreateOperation(transitStack, null);

			Assert.That(operation.Execute(), Is.True);
			Assert.That(slot.HasStack(), Is.True);
			Assert.That(slot.GetStack(), Is.EqualTo(transitStack));
			Assert.That(scope.HasTransitStack(), Is.False);

			scope.Received().SetTransitStack(Arg.Is((ItemStack)null));
			slot.Received().SetStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_CannotExecute_ReturnsFalse() {
			CreateOperation(null, null);

			Assert.That(operation.Execute(), Is.False);

			scope.DidNotReceive().SetTransitStack(Arg.Any<ItemStack>());
			slot.DidNotReceive().SetStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_SlotFullNoSpace_ReturnsFalse() {
			CreateOperation(fakeItem.CreateStack(), fakeItem.CreateStack(fakeItem.fullStackSize));

			Assert.That(operation.Execute(), Is.False);

			scope.DidNotReceive().SetTransitStack(Arg.Any<ItemStack>());
			slot.DidNotReceive().SetStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_PartialTransfer_WhenSlotSpaceIsInsufficient() {
			fakeItem._fullStackSize = 10;
			CreateOperation(fakeItem.CreateStack(5), fakeItem.CreateStack(8));

			Assert.That(operation.Execute(), Is.True);
			Assert.That(scope.HasTransitStack(), Is.True);
			Assert.That(scope.GetTransitStack().quantity, Is.EqualTo(3));
			Assert.That(slot.GetStack().IsFull(), Is.True);
		}

		[Test]
		public void Execute_FullSpace_TransfersFullTransitQuantity() {
			CreateOperation(fakeItem.CreateStack(10), null);

			Assert.That(operation.Execute(), Is.True);
			Assert.That(scope.HasTransitStack(), Is.False);
			Assert.That(slot.HasStack(), Is.True);
			Assert.That(slot.GetStack().quantity, Is.EqualTo(10));
		}

		[Test]
		public void Execute_TransfersCorrectAmount_IncrementsSlotStack_DecrementsTransitStack() {
			fakeItem._fullStackSize = 10;
			ItemStack slotStack = fakeItem.CreateStack(7);
			ItemStack transitStack = fakeItem.CreateStack(5);
			CreateOperation(transitStack, slotStack);

			Assert.That(operation.Execute(), Is.True);
			Assert.That(slotStack.quantity, Is.EqualTo(10));
			Assert.That(transitStack.quantity, Is.EqualTo(2));

			scope.DidNotReceive().SetTransitStack(Arg.Any<ItemStack>());
			slot.DidNotReceive().SetStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_TransfersFullTransit_WhenTransitQuantityIsEqualToSpaceLeft() {
			fakeItem._fullStackSize = 10;
			CreateOperation(fakeItem.CreateStack(4), fakeItem.CreateStack(6));

			Assert.That(operation.Execute(), Is.True);
			Assert.That(scope.HasTransitStack(), Is.False);
			Assert.That(slot.GetStack().IsFull(), Is.True);
		}

	}
}
