using NSubstitute;
using NUnit.Framework;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.ItemSystem.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemTests.Container.Operations {
	internal class TransferSingleToSlotTests : SingleSlotOperationTests<TransferSingleToSlot> {
		protected override TransferSingleToSlot GetOperation(IItemContainer container, int slotIndex, IItemContainerScope scope) {
			return new TransferSingleToSlot(container, slotIndex, scope);
		}

		[Test]
		public void CanExecute_SlotEmpty_NoTransitStack_ReturnsFalse() {
			CreateOperation(null, null);

			Assert.That(operation.CanExecute(), Is.False);
		}

		[Test]
		public void CanExecute_SlotEmpty_HasTransitStack_ReturnsTrue() {
			CreateOperation(fakeItem.CreateStack(), null);

			Assert.That(operation.CanExecute(), Is.True);
		}

		[Test]
		public void CanExecute_SlotOccupied_StackableWithTransit_ReturnsTrue() {
			CreateOperation(fakeItem.CreateStack(), fakeItem.CreateStack());

			Assert.That(operation.CanExecute(), Is.True);
		}

		[Test]
		public void CanExecute_SlotOccupied_NotStackableWithTransit_ReturnsFalse() {
			Item other = new FakeItem() {
				_fullStackSize = DEFAULT_FULL_STACK
			};
			CreateOperation(other.CreateStack(), fakeItem.CreateStack());

			Assert.That(operation.CanExecute(), Is.False);
		}

		[Test]
		public void Execute_WhenCannotExecute_ReturnsFalseWithNoSideEffects() {
			CreateOperation(null, null);

			Assert.That(operation.Execute(), Is.False);
			Assert.That(slot.HasStack(), Is.False);
			Assert.That(scope.HasTransitStack(), Is.False);

			slot.DidNotReceive().SetStack(Arg.Any<ItemStack>());
			scope.DidNotReceive().SetTransitStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_SlotEmpty_ClonesOneItemIntoSlot() {
			CreateOperation(fakeItem.CreateStack(), null);

			Assert.That(operation.Execute(), Is.True);
			Assert.That(slot.HasStack(), Is.True);
			Assert.That(slot.GetStack().quantity, Is.EqualTo(1));

			slot.Received().SetStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_SlotEmpty_DecrementsTransitStack() {
			CreateOperation(fakeItem.CreateStack(5), null);

			Assert.That(operation.Execute(), Is.True);
			Assert.That(scope.HasTransitStack(), Is.True);
			Assert.That(scope.GetTransitStack().quantity, Is.EqualTo(4));

			scope.DidNotReceive().SetTransitStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_SlotEmpty_ReturnsTrue() {
			CreateOperation(fakeItem.CreateStack(), null);

			Assert.That(operation.Execute(), Is.True);
		}

		[Test]
		public void Execute_SlotOccupied_IncrementsSlotStack() {
			CreateOperation(fakeItem.CreateStack(), fakeItem.CreateStack(4));

			Assert.That(operation.Execute(), Is.True);
			Assert.That(slot.HasStack(), Is.True);
			Assert.That(slot.GetStack().quantity, Is.EqualTo(5));

			slot.DidNotReceive().SetStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_SlotOccupied_DecrementsTransit() {
			CreateOperation(fakeItem.CreateStack(6), fakeItem.CreateStack());

			Assert.That(operation.Execute(), Is.True);
			Assert.That(scope.HasTransitStack(), Is.True);
			Assert.That(scope.GetTransitStack().quantity, Is.EqualTo(5));

			scope.DidNotReceive().SetTransitStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_SlotOccupied_StackFull_DoesNotDecrementTransit() {
			CreateOperation(fakeItem.CreateStack(5), fakeItem.CreateStack(fakeItem.fullStackSize));

			Assert.That(operation.Execute(), Is.True);
			Assert.That(scope.HasTransitStack(), Is.True);
			Assert.That(scope.GetTransitStack().quantity, Is.EqualTo(5));

			scope.DidNotReceive().SetTransitStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_SlotOccupied_StackFull_ReturnsTrue() {
			CreateOperation(fakeItem.CreateStack(5), fakeItem.CreateStack(fakeItem.fullStackSize));

			Assert.That(operation.Execute(), Is.True);
		}


	}
}
