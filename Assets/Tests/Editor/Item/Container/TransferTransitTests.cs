using NSubstitute;
using NUnit.Framework;
using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.ItemSystem.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemTests.Container.Operations {
	internal class TransferTransitTests : SingleSlotOperationTests<TransferTransit> {
		protected override TransferTransit GetOperation(IItemContainer container, int slotIndex, IItemContainerScope scope) {
			return new TransferTransit(container, slotIndex, scope);
		}

		[Test]
		public void CanExecute_SlotHasStack_ReturnsTrue() {
			CreateOperation(null, fakeItem.CreateStack());

			Assert.That(operation.CanExecute(), Is.True);
		}

		[Test]
		public void CanExecute_ScopeHasTransitStack_ReturnsTrue() {
			CreateOperation(fakeItem.CreateStack(), null);

			Assert.That(operation.CanExecute(), Is.True);
		}


		[Test]
		public void CanExecute_BothHaveStacks_ReturnsTrue() {
			CreateOperation(fakeItem.CreateStack(), fakeItem.CreateStack());

			Assert.That(operation.CanExecute(), Is.True);
		}

		[Test]
		public void CanExecute_NeitherHasStack_ReturnsFalse() {
			CreateOperation(null, null);

			Assert.That(operation.CanExecute(), Is.False);
		}


		[Test]
		public void CanExecute_BothHaveStacks_DifferentItems_ReturnsTrue() {
			Item other = new FakeItem() {
				_fullStackSize = DEFAULT_FULL_STACK
			};
			CreateOperation(other.CreateStack(), fakeItem.CreateStack());

			Assert.That(operation.CanExecute(), Is.True);
		}

		[Test]
		public void Execute_CannotExecute_ReturnsFalse() {
			CreateOperation(null, null);

			Assert.That(operation.Execute(), Is.False);
		}

		[Test]
		public void Execute_SlotHasStack_TransitHasStack_MergesAndReturnsTrue() {
			CreateOperation(fakeItem.CreateStack(5), fakeItem.CreateStack(10));

			Assert.That(operation.Execute(), Is.True);
			Assert.That(scope.HasTransitStack(), Is.False);
			Assert.That(slot.HasStack(), Is.True);
			Assert.That(slot.GetStack().quantity, Is.EqualTo(15));

			scope.Received().SetTransitStack(Arg.Is((ItemStack)null));
			slot.DidNotReceive().SetStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_SlotHasStack_NoTransitStack_GrabsAndReturnsTrue() {
			ItemStack slotStack = fakeItem.CreateStack(10);
			CreateOperation(null, slotStack);

			Assert.That(operation.Execute(), Is.True);
			Assert.That(scope.HasTransitStack(), Is.True);
			Assert.That(slot.HasStack(), Is.False);

			scope.Received().SetTransitStack(Arg.Is(slotStack));
			slot.Received().SetStack(Arg.Is((ItemStack)null));
		}

		[Test]
		public void Execute_SlotHasStack_TransitHasStack_DifferentItems_SwapsAndReturnsTrue() {
			Item other = new FakeItem() {
				_fullStackSize = DEFAULT_FULL_STACK
			};
			ItemStack transitStack = other.CreateStack();
			ItemStack slotStack = fakeItem.CreateStack();
			CreateOperation(transitStack, slotStack);

			Assert.That(operation.Execute(), Is.True);
			Assert.That(slot.HasStack(), Is.True);
			Assert.That(scope.HasTransitStack(), Is.True);

			scope.Received().SetTransitStack(Arg.Is(slotStack));
			slot.Received().SetStack(Arg.Is(transitStack));
		}

		[Test]
		public void Execute_NoSubOperationApplies_ReturnsFalse() {
			CreateOperation(null, null);

			Assert.That(operation.Execute(), Is.False);
		}
	}
}
