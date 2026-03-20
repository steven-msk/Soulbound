using NSubstitute;
using NUnit.Framework;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.ItemSystem.Container;
using System.Collections.Generic;

namespace ItemTests.Container.Operations {
	public class CollectAllItemsToTransitTests {
		const int DEFAULT_FULL_STACK = 256;
		private ISlotOperation operation;
		private IItemContainerScope scope;
		private FakeItem fakeItem;

		[SetUp]
		public void Setup() {
			scope = Substitute.For<IItemContainerScope>();
			fakeItem = new FakeItem {
				_fullStackSize = DEFAULT_FULL_STACK
			};
		}

		private void CreateOperation(ItemStack? transitStack) {
			scope.GetTransitStack().Returns(transitStack);
			scope.HasTransitStack().Returns(transitStack != null);

			operation = new CollectAllItemsToTransit(scope);
		}


		[Test]
		public void CanExecute_ReturnsFalse_WhenNoTransitStack() {
			CreateOperation(null);
			Assert.That(operation.CanExecute(), Is.False);
		}

		[Test]
		public void CanExecute_ReturnsFalse_WhenTransitStackIsFull() {
			ItemStack stack = fakeItem.CreateStack(fakeItem.fullStackSize);
			CreateOperation(stack);
			Assert.That(operation.CanExecute(), Is.False);
		}

		[Test]
		public void CanExecute_ReturnsTrue_WhenTransitStackExistsAndNotFull() {
			ItemStack stack = fakeItem.CreateStack(3);
			CreateOperation(stack);
			Assert.That(operation.CanExecute(), Is.True);
		}

		[Test]
		public void Execute_ReturnsFalse_WhenCanExecuteIsFalse() {
			CreateOperation(null);
			Assert.That(operation.Execute(), Is.False);
		}

		[Test]
		public void Execute_ReturnsFalse_WhenNoSlotsContainTransitItem() {
			ItemStack transitStack = fakeItem.CreateStack(4);
			CreateOperation(transitStack);

			FakeItem item1 = Substitute.For<FakeItem>();
			FakeItem item2 = Substitute.For<FakeItem>();
			FakeItem item3 = Substitute.For<FakeItem>();

			IEnumerable<IItemContainer> containers = new IItemContainer[] {
				ContainerUtils.SubstituteContainer(
					item1.CreateStack(1),
					item2.CreateStack(2),
					item3.CreateStack(2),
					item2.CreateStack(50),
					null
				),
				ContainerUtils.SubstituteContainer(
					item1.CreateStack(100),
					item2.CreateStack(15)
				)
			};
			scope.GetOpenContainers().Returns(containers);

			Assert.That(operation.Execute(), Is.False);
		}

		[Test]
		public void Execute_ReturnsTrue_WhenItemsSuccessfullyCollected() {
			ItemStack transitStack = fakeItem.CreateStack(4);
			CreateOperation(transitStack);

			FakeItem item1 = Substitute.For<FakeItem>();
			FakeItem item2 = Substitute.For<FakeItem>();

			IEnumerable<IItemContainer> containers = new IItemContainer[] {
				ContainerUtils.SubstituteContainer(
					fakeItem.CreateStack(5),
					fakeItem.CreateStack(4),
					item2.CreateStack(53),
					null,
					item1.CreateStack(5)
				)
			};
			scope.GetOpenContainers().Returns(containers);

			Assert.That(operation.Execute(), Is.True);
			Assert.That(transitStack.quantity, Is.EqualTo(4 + 5 + 4));
		}

		[Test]
		public void Execute_CollectsFromMultipleContainers() {
			ItemStack transitStack = fakeItem.CreateStack(4);
			CreateOperation(transitStack);

			IEnumerable<IItemContainer> containers = new IItemContainer[] {
				ContainerUtils.SubstituteContainer(
					fakeItem.CreateStack(5),
					fakeItem.CreateStack(4)
				),
				ContainerUtils.SubstituteContainer(
					fakeItem.CreateStack(65),
					fakeItem.CreateStack(12),
					fakeItem.CreateStack(3)
				)
			};
			scope.GetOpenContainers().Returns(containers);

			Assert.That(operation.Execute(), Is.True);
			Assert.That(transitStack.quantity, Is.EqualTo(4 + 5 + 4 + 65 + 12 + 3));
		}

		[Test]
		public void Execute_RespectsTransitStackCapacity_WhenSlotsExceedSpace() {
			const int maxStackSize = 10;
			fakeItem._fullStackSize = maxStackSize;
			ItemStack transitStack = fakeItem.CreateStack(4);
			CreateOperation(transitStack);

			IEnumerable<IItemContainer> containers = new IItemContainer[] {
				ContainerUtils.SubstituteContainer(
					fakeItem.CreateStack(3),
					fakeItem.CreateStack(2)
				),
				ContainerUtils.SubstituteContainer(
					fakeItem.CreateStack(3)	// should be 4 + 3 + 2 + 3 = 12 total
				)
			};
			scope.GetOpenContainers().Returns(containers);

			Assert.That(operation.Execute(), Is.True);
			Assert.That(transitStack.quantity, Is.EqualTo(maxStackSize));
		}

		[Test]
		public void Execute_SlotsAreProcessedInAscendingQuantityOrder() {
			ItemStack transitStack = fakeItem.CreateStack();
			CreateOperation(transitStack);

			List<int> order = new();
			ItemStack NewStack(int quantity, int id) {
				ItemStack stack = fakeItem.CreateStack(quantity);
				stack.onQuantityChanged += (_, _) => order.Add(id);
				return stack;
			}

			IItemContainer container = ContainerUtils.SubstituteContainer(
				NewStack(5, 0),
				NewStack(3, 1),
				NewStack(6, 2),
				NewStack(8, 3),
				NewStack(2, 4)
			);
			IEnumerable<IItemContainer> containers = new IItemContainer[] { container };
			scope.GetOpenContainers().Returns(containers);

			Assert.That(operation.Execute(), Is.True);

			int[] expectedOrder = new int[] { 4, 1, 0, 2, 3 };
			Assert.That(order.Count, Is.EqualTo(expectedOrder.Length));
			CollectionAssert.AreEqual(expectedOrder, order);
		}

		[Test]
		public void Execute_PartiallyFillsTransitStack_WhenNotEnoughItems() {
			const int fullStackSize = 10;
			fakeItem._fullStackSize = fullStackSize;
			ItemStack transitStack = fakeItem.CreateStack();
			CreateOperation(transitStack);

			IEnumerable<IItemContainer> containers = new IItemContainer[] {
				ContainerUtils.SubstituteContainer(
					fakeItem.CreateStack(),
					fakeItem.CreateStack(5),
					fakeItem.CreateStack(2)
				)
			};
			scope.GetOpenContainers().Returns(containers);

			Assert.That(operation.Execute(), Is.True);
			Assert.That(transitStack.quantity, Is.LessThan(fullStackSize));
		}

		[Test]
		public void Execute_ExactlyFillsTransitStack_WhenItemsMatchRemainingSpace() {
			const int fullStackSize = 10;
			fakeItem._fullStackSize = fullStackSize;
			ItemStack transitStack = fakeItem.CreateStack();
			CreateOperation(transitStack);

			IEnumerable<IItemContainer> containers = new IItemContainer[] {
				ContainerUtils.SubstituteContainer(
					fakeItem.CreateStack(),
					fakeItem.CreateStack(2),
					fakeItem.CreateStack(3),
					fakeItem.CreateStack(3)
				)
			};
			scope.GetOpenContainers().Returns(containers);

			Assert.That(operation.Execute(), Is.True);
			Assert.That(transitStack.quantity, Is.EqualTo(fullStackSize));
		}

		[Test]
		public void Execute_DecreasesSourceSlotQuantity_ByTransferredAmount() {
			const int fullStackSize = 10;
			fakeItem._fullStackSize = fullStackSize;
			ItemStack transitStack = fakeItem.CreateStack(5);
			CreateOperation(transitStack);

			ItemStack stack = fakeItem.CreateStack(7);
			const int shouldRemain = 2;

			IEnumerable<IItemContainer> containers = new IItemContainer[] {
				ContainerUtils.SubstituteContainer(stack)
			};
			scope.GetOpenContainers().Returns(containers);

			Assert.That(operation.Execute(), Is.True);
			Assert.That(transitStack.quantity, Is.EqualTo(fullStackSize));
			Assert.That(stack.quantity, Is.EqualTo(shouldRemain));
		}

		[Test]
		public void Execute_DoesNotModifySlots_WhenTransitStackAlreadyFull() {
			const int fullStackSize = 10;
			fakeItem._fullStackSize = fullStackSize;
			ItemStack transitStack = fakeItem.CreateStack(fullStackSize);
			CreateOperation(transitStack);

			ItemStack stack1 = fakeItem.CreateStack();
			ItemStack stack2 = fakeItem.CreateStack(6);
			ItemStack stack3 = fakeItem.CreateStack(2);

			IEnumerable<IItemContainer> containers = new IItemContainer[] {
				ContainerUtils.SubstituteContainer(stack1, stack2, stack3)
			};
			scope.GetOpenContainers().Returns(containers);

			Assert.That(operation.Execute(), Is.False);
			Assert.That(transitStack.quantity, Is.EqualTo(fullStackSize));
			Assert.That(stack1.quantity, Is.EqualTo(1));
			Assert.That(stack2.quantity, Is.EqualTo(6));
			Assert.That(stack3.quantity, Is.EqualTo(2));
		}

		[Test]
		public void Execute_HandlesMultipleSlotsAcrossMultipleContainers() {
			ItemStack transitStack = fakeItem.CreateStack();
			CreateOperation(transitStack);

			ItemStack stack1 = fakeItem.CreateStack();
			ItemStack stack2 = fakeItem.CreateStack();

			IItemContainer container1 = ContainerUtils.SubstituteContainer(stack1);
			IItemContainer container2 = ContainerUtils.SubstituteContainer(stack2);
			IEnumerable<IItemContainer> containers = new IItemContainer[] { container1, container2 };
			scope.GetOpenContainers().Returns(containers);

			Assert.That(operation.Execute(), Is.True);
			Assert.That(stack1.IsEmpty(), Is.True);
			Assert.That(stack2.IsEmpty(), Is.True);
		}
	}
}
