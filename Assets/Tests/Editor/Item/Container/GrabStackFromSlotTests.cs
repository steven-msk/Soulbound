using NSubstitute;
using NUnit.Framework;
using SoulboundBackend.Client.Debug.Logging;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.ItemSystem.Container;
using System;
using System.Collections.Generic;

#nullable enable

namespace ItemTests.Container.Operations {
	public class GrabStackFromSlotTests {
		private const int DEFAULT_FULL_STACK = 256;
		private IItemContainerScope scope;
		private FakeItem fakeItem;
		private ISlotOperation operation;
		private IItemSlot slot;

		[SetUp]
		public void Setup() {
			scope = Substitute.For<IItemContainerScope>();
			fakeItem = new FakeItem {
				_fullStackSize = DEFAULT_FULL_STACK,
			};
		}

		private void CreateOperation(ItemStack? transitStack, ItemStack? slotStack) {
			IItemContainer container = Substitute.For<IItemContainer>();

			slot = Substitute.For<IItemSlot>();
			slot.GetStack().Returns(slotStack);
			slot.HasStack().Returns(slotStack != null);
			slot.When(s => s.SetStack(Arg.Any<ItemStack>())).Do(callInfo => {
				ItemStack stack = callInfo.Arg<ItemStack>();
				slot.GetStack().Returns(stack);
				slot.HasStack().Returns(stack != null);
			});
			slot.GetIndex().Returns(0);
			slot.GetContainer().Returns(container);

			IReadOnlyList<int> slots = new int[] { 0 };
			container.GetAllSlots().Returns(slots);
			container.GetSlotCount().Returns(1);
			container.GetSlot(Arg.Is(0)).Returns(slot);

			scope.GetTransitStack().Returns(transitStack);
			scope.HasTransitStack().Returns(transitStack != null);
			scope.When(s => s.SetTransitStack(Arg.Any<ItemStack>())).Do(callInfo => {
				ItemStack stack = callInfo.Arg<ItemStack>();
				scope.GetTransitStack().Returns(stack);
				scope.HasTransitStack().Returns(stack != null);
			});

			IEnumerable<IItemContainer> containers = new IItemContainer[] { container };
			scope.GetOpenContainers().Returns(containers);

			operation = new GrabStackFromSlot(container, 0, scope);
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
			Assert.That(operation.CanExecute(), Is.True);
			Assert.That(operation.Execute(), Is.True);
		}

		[Test]
		public void Execute_WhenCannotExecute_ReturnsFalse() {
			CreateOperation(null, null);
			Assert.That(operation.CanExecute(), Is.False);
			Assert.That(operation.Execute(), Is.False);
		}

		[Test]
		public void Execute_WhenCanExecute_SetsTransitStackToSlotStack() {
			ItemStack slotStack = fakeItem.CreateStack();
			CreateOperation(null, slotStack);

			Assert.That(operation.CanExecute(), Is.True);
			Assert.That(operation.Execute(), Is.True);
			Assert.That(scope.GetTransitStack(), Is.EqualTo(slotStack));
		}

		[Test]
		public void Execute_WhenCanExecute_ClearsSlotStack() {
			CreateOperation(null, fakeItem.CreateStack());

			Assert.That(operation.CanExecute(), Is.True);
			Assert.That(operation.Execute(), Is.True);
			Assert.That(slot.HasStack(), Is.False);
		}

		[Test]
		public void Execute_WhenCannotExecute_DoesNotModifyScope() {
			ItemStack transitStack = fakeItem.CreateStack();
			CreateOperation(transitStack, null);

			Assert.That(operation.CanExecute(), Is.False);
			Assert.That(operation.Execute(), Is.False);
			Assert.That(scope.HasTransitStack(), Is.True);
			Assert.That(scope.GetTransitStack(), Is.EqualTo(transitStack));
		}

		[Test]
		public void Execute_WhenCannotExecute_DoesNotModifySlot() {
			ItemStack slotStack = fakeItem.CreateStack();
			CreateOperation(fakeItem.CreateStack(), slotStack);

			Assert.That(operation.CanExecute(), Is.False);
			Assert.That(operation.Execute(), Is.False);
			Assert.That(slot.HasStack(), Is.True);
			Assert.That(slot.GetStack(), Is.EqualTo(slotStack));
		}

		[Test]
		public void Execute_WhenCanExecute_SetsTransitStackAndClearsSlot() {
			ItemStack slotStack = fakeItem.CreateStack();
			CreateOperation(null, slotStack);

			Assert.That(operation.CanExecute(), Is.True);
			Assert.That(operation.Execute(), Is.True);
			Assert.That(scope.GetTransitStack(), Is.EqualTo(slotStack));
			Assert.That(slot.HasStack(), Is.False);
		}
	}
}
