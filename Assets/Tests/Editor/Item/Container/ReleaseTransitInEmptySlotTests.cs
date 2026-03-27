
using NSubstitute;
using NUnit.Framework;
using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.ItemSystem.Container;

namespace ItemTests.Container.Operations {
	internal class ReleaseTransitInEmptySlotTests : SingleSlotOperationTests<ReleaseTransitInEmptySlot> {
		protected override ReleaseTransitInEmptySlot GetOperation(IItemContainer container, int slotIndex, IItemContainerScope scope) {
			return new ReleaseTransitInEmptySlot(container, slotIndex, scope);
		}

		[Test]
		public void CanExecute_SlotEmptyAndTransitHasStack_ReturnsTrue() {
			CreateOperation(fakeItem.CreateStack(), null);

			Assert.That(operation.CanExecute(), Is.True);
		}

		[Test]
		public void CanExecute_SlotHasStack_ReturnsFalse() {
			CreateOperation(fakeItem.CreateStack(), fakeItem.CreateStack());

			Assert.That(operation.CanExecute(), Is.False);
		}

		[Test]
		public void CanExecute_NoTransitStack_ReturnsFalse() {
			CreateOperation(null, null);

			Assert.That(operation.CanExecute(), Is.False);
		}

		[Test]
		public void CanExecute_SlotHasStackAndNoTransit_ReturnsFalse() {
			CreateOperation(null, fakeItem.CreateStack());

			Assert.That(operation.CanExecute(), Is.False);
		}

		[Test]
		public void Execute_ValidConditions_SetsSlotToTransitStack() {
			ItemStack transitStack = fakeItem.CreateStack();
			CreateOperation(transitStack, null);

			Assert.That(operation.Execute(), Is.True);
			Assert.That(slot.HasStack(), Is.True);
			Assert.That(slot.GetStack(), Is.EqualTo(transitStack));

			slot.Received().SetStack(Arg.Is(transitStack));
			scope.Received().SetTransitStack(Arg.Is((ItemStack)null));
		}

		[Test]
		public void Execute_ValidConditions_ClearsTransitStack() {
			CreateOperation(fakeItem.CreateStack(), null);

			Assert.That(operation.Execute(), Is.True);
			Assert.That(scope.HasTransitStack(), Is.False);

			scope.Received().SetTransitStack(Arg.Is((ItemStack)null));
		}

		[Test]
		public void Execute_SlotNotEmpty_ReturnsFalse() {
			CreateOperation(fakeItem.CreateStack(), fakeItem.CreateStack());

			Assert.That(operation.Execute(), Is.False);

			scope.DidNotReceive().SetTransitStack(Arg.Any<ItemStack>());
			slot.DidNotReceive().SetStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_NoTransitStack_ReturnsFalse() {
			CreateOperation(null, null);

			Assert.That(operation.Execute(), Is.False);

			scope.DidNotReceive().SetTransitStack(Arg.Any<ItemStack>());
			slot.DidNotReceive().SetStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_SlotNotEmpty_DoesNotModifySlot() {
			ItemStack slotStack = fakeItem.CreateStack();
			CreateOperation(fakeItem.CreateStack(), slotStack);

			Assert.That(operation.Execute(), Is.False);
			Assert.That(slot.HasStack(), Is.True);
			Assert.That(slot.GetStack(), Is.EqualTo(slotStack));

			scope.DidNotReceive().SetTransitStack(Arg.Any<ItemStack>());
			slot.DidNotReceive().SetStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_NoTransitStack_DoesNotModifySlot() {
			ItemStack slotStack = fakeItem.CreateStack();
			CreateOperation(null, slotStack);

			Assert.That(operation.Execute(), Is.False);
			Assert.That(slot.HasStack(), Is.True);
			Assert.That(slot.GetStack(), Is.EqualTo(slotStack));

			scope.DidNotReceive().SetTransitStack(Arg.Any<ItemStack>());
			slot.DidNotReceive().SetStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_SlotNotEmpty_DoesNotModifyTransitStack() {
			ItemStack transitStack = fakeItem.CreateStack();
			CreateOperation(transitStack, fakeItem.CreateStack());

			Assert.That(operation.Execute(), Is.False);
			Assert.That(scope.HasTransitStack(), Is.True);
			Assert.That(scope.GetTransitStack(), Is.EqualTo(transitStack));

			scope.DidNotReceive().SetTransitStack(Arg.Any<ItemStack>());
			slot.DidNotReceive().SetStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_NoTransitStack_DoesNotModifyTransitStack() {
			CreateOperation(null, null);

			Assert.That(operation.Execute(), Is.False);
			Assert.That(scope.HasTransitStack(), Is.False);

			scope.DidNotReceive().SetTransitStack(Arg.Any<ItemStack>());
			slot.DidNotReceive().SetStack(Arg.Any<ItemStack>());
		}
	}
}
