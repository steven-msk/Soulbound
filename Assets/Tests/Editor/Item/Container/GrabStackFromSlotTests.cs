using NSubstitute;
using NUnit.Framework;
using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.ItemSystem.Container;
using System;
using System.Collections.Generic;

#nullable enable

namespace ItemTests.Container.Operations {
	internal class GrabStackFromSlotTests : SingleSlotOperationTests<GrabStackFromSlot> {
		protected override GrabStackFromSlot GetOperation(IItemContainer container, int slotIndex, IItemContainerScope scope) {
			return new GrabStackFromSlot(container, slotIndex, scope);
		}

		[Test]
		public void CanExecute_SlotHasStackAndScopeHasNoTransitStack_ReturnsTrue() {
			CreateOperation(null, fakeItem.CreateStack());

			Assert.That(operation.CanExecute(), Is.True);
		}

		[Test]
		public void CanExecute_SlotHasNoStack_ReturnsFalse() {
			CreateOperation(null, null);

			Assert.That(operation.CanExecute(), Is.False);
		}

		[Test]
		public void CanExecute_ScopeAlreadyHasTransitStack_ReturnsFalse() {
			CreateOperation(fakeItem.CreateStack(), fakeItem.CreateStack());

			Assert.That(operation.CanExecute(), Is.False);
		}

		[Test]
		public void CanExecute_SlotHasNoStackAndScopeHasTransitStack_ReturnsFalse() {
			CreateOperation(fakeItem.CreateStack(), null);

			Assert.That(operation.CanExecute(), Is.False);
		}

		[Test]
		public void Execute_WhenCanExecute_ReturnsTrue() {
			CreateOperation(null, fakeItem.CreateStack());

			Assert.That(operation.Execute(), Is.True);

			scope.Received().SetTransitStack(Arg.Any<ItemStack>());
			slot.Received().SetStack(Arg.Is((ItemStack)null));
		}

		[Test]
		public void Execute_WhenCannotExecute_ReturnsFalse() {
			CreateOperation(null, null);

			Assert.That(operation.Execute(), Is.False);

			scope.DidNotReceive().SetTransitStack(Arg.Any<ItemStack>());
			slot.DidNotReceive().SetStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_WhenCanExecute_SetsTransitStackToSlotStack() {
			ItemStack slotStack = fakeItem.CreateStack();
			CreateOperation(null, slotStack);

			Assert.That(operation.Execute(), Is.True);
			Assert.That(scope.GetTransitStack(), Is.EqualTo(slotStack));
		}

		[Test]
		public void Execute_WhenCanExecute_ClearsSlotStack() {
			CreateOperation(null, fakeItem.CreateStack());

			Assert.That(operation.Execute(), Is.True);
			Assert.That(slot.HasStack(), Is.False);
		}

		[Test]
		public void Execute_WhenCannotExecute_DoesNotModifyScope() {
			ItemStack transitStack = fakeItem.CreateStack();
			CreateOperation(transitStack, null);

			Assert.That(operation.Execute(), Is.False);
			Assert.That(scope.HasTransitStack(), Is.True);
			Assert.That(scope.GetTransitStack(), Is.EqualTo(transitStack));

			scope.DidNotReceive().SetTransitStack(Arg.Any<ItemStack>());
			slot.DidNotReceive().SetStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_WhenCannotExecute_DoesNotModifySlot() {
			ItemStack slotStack = fakeItem.CreateStack();
			CreateOperation(fakeItem.CreateStack(), slotStack);

			Assert.That(operation.Execute(), Is.False);
			Assert.That(slot.HasStack(), Is.True);
			Assert.That(slot.GetStack(), Is.EqualTo(slotStack));

			scope.DidNotReceive().SetTransitStack(Arg.Any<ItemStack>());
			slot.DidNotReceive().SetStack(Arg.Any<ItemStack>());
		}

		[Test]
		public void Execute_WhenCanExecute_SetsTransitStackAndClearsSlot() {
			ItemStack slotStack = fakeItem.CreateStack();
			CreateOperation(null, slotStack);

			Assert.That(operation.Execute(), Is.True);
			Assert.That(scope.GetTransitStack(), Is.EqualTo(slotStack));
			Assert.That(slot.HasStack(), Is.False);

			scope.Received().SetTransitStack(Arg.Any<ItemStack>());
			slot.Received().SetStack(Arg.Is((ItemStack)null));
		}
	}
}
