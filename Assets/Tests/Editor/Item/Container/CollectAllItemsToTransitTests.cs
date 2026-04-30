using NSubstitute;
using NUnit.Framework;
using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.ItemSystem.Container;
using System.Collections.Generic;

namespace ItemTests.Container.Operations {
	internal class CollectAllItemsToTransitTests : SlotOperationTest {
		private void CreateOperation(ItemStack? transitStack) {
			this.scope.GetTransitStack().Returns(transitStack);
			this.scope.HasTransitStack().Returns(transitStack != null);

			this.operation = new CollectAllItemsToTransit(this.scope);
		}

		[Test]
		public void CanExecute_ReturnsFalse_WhenNoTransitStack() {
			this.CreateOperation(null);

			Assert.That(this.operation.CanExecute(), Is.False);
		}

		[Test]
		public void CanExecute_ReturnsFalse_WhenTransitStackIsFull() {
			ItemStack stack = this.fakeItem.CreateStack(this.fakeItem.fullStackSize);
			this.CreateOperation(stack);

			Assert.That(this.operation.CanExecute(), Is.False);
		}

		[Test]
		public void CanExecute_ReturnsTrue_WhenTransitStackExistsAndNotFull() {
			ItemStack stack = this.fakeItem.CreateStack(3);
			this.CreateOperation(stack);

			Assert.That(this.operation.CanExecute(), Is.True);
		}

		[Test]
		public void Execute_ReturnsFalse_WhenCanExecuteIsFalse() {
			this.CreateOperation(null);

			Assert.That(this.operation.Execute(), Is.False);
		}

		[Test]
		public void Execute_ReturnsFalse_WhenNoSlotsContainTransitItem() {
			ItemStack transitStack = this.fakeItem.CreateStack(4);
			this.CreateOperation(transitStack);

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
			this.scope.GetOpenContainers().Returns(containers);

			Assert.That(this.operation.Execute(), Is.False);
		}

		[Test]
		public void Execute_ReturnsTrue_WhenItemsSuccessfullyCollected() {
			ItemStack transitStack = this.fakeItem.CreateStack(4);
			this.CreateOperation(transitStack);

			FakeItem item1 = Substitute.For<FakeItem>();
			FakeItem item2 = Substitute.For<FakeItem>();

			IEnumerable<IItemContainer> containers = new IItemContainer[] {
				ContainerUtils.SubstituteContainer(
					this.fakeItem.CreateStack(5),
					this.fakeItem.CreateStack(4),
					item2.CreateStack(53),
					null,
					item1.CreateStack(5)
				)
			};
			this.scope.GetOpenContainers().Returns(containers);

			Assert.That(this.operation.Execute(), Is.True);
			Assert.That(transitStack.quantity, Is.EqualTo(4 + 5 + 4));
		}

		[Test]
		public void Execute_CollectsFromMultipleContainers() {
			ItemStack transitStack = this.fakeItem.CreateStack(4);
			this.CreateOperation(transitStack);

			IEnumerable<IItemContainer> containers = new IItemContainer[] {
				ContainerUtils.SubstituteContainer(
					this.fakeItem.CreateStack(5),
					this.fakeItem.CreateStack(4)
				),
				ContainerUtils.SubstituteContainer(
					this.fakeItem.CreateStack(65),
					this.fakeItem.CreateStack(12),
					this.fakeItem.CreateStack(3)
				)
			};
			this.scope.GetOpenContainers().Returns(containers);

			Assert.That(this.operation.Execute(), Is.True);
			Assert.That(transitStack.quantity, Is.EqualTo(4 + 5 + 4 + 65 + 12 + 3));
		}

		[Test]
		public void Execute_RespectsTransitStackCapacity_WhenSlotsExceedSpace() {
			const int maxStackSize = 10;
			this.fakeItem = new FakeItem(maxStackSize);
			ItemStack transitStack = this.fakeItem.CreateStack(4);
			this.CreateOperation(transitStack);

			IEnumerable<IItemContainer> containers = new IItemContainer[] {
				ContainerUtils.SubstituteContainer(
					this.fakeItem.CreateStack(3),
					this.fakeItem.CreateStack(2)
				),
				ContainerUtils.SubstituteContainer(
					this.fakeItem.CreateStack(3)	// should be 4 + 3 + 2 + 3 = 12 total
				)
			};
			this.scope.GetOpenContainers().Returns(containers);

			Assert.That(this.operation.Execute(), Is.True);
			Assert.That(transitStack.quantity, Is.EqualTo(maxStackSize));
		}

		[Test]
		public void Execute_SlotsAreProcessedInAscendingQuantityOrder() {
			ItemStack transitStack = this.fakeItem.CreateStack();
			this.CreateOperation(transitStack);

			List<int> order = new();
			ItemStack NewStack(int quantity, int id) {
				ItemStack stack = this.fakeItem.CreateStack(quantity);
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
			this.scope.GetOpenContainers().Returns(containers);

			Assert.That(this.operation.Execute(), Is.True);

			int[] expectedOrder = new int[] { 4, 1, 0, 2, 3 };
			Assert.That(order.Count, Is.EqualTo(expectedOrder.Length));
			CollectionAssert.AreEqual(expectedOrder, order);
		}

		[Test]
		public void Execute_PartiallyFillsTransitStack_WhenNotEnoughItems() {
			const int fullStackSize = 10;
			this.fakeItem = new FakeItem(fullStackSize);
			ItemStack transitStack = this.fakeItem.CreateStack();
			this.CreateOperation(transitStack);

			IEnumerable<IItemContainer> containers = new IItemContainer[] {
				ContainerUtils.SubstituteContainer(
					this.fakeItem.CreateStack(),
					this.fakeItem.CreateStack(5),
					this.fakeItem.CreateStack(2)
				)
			};
			this.scope.GetOpenContainers().Returns(containers);

			Assert.That(this.operation.Execute(), Is.True);
			Assert.That(transitStack.quantity, Is.LessThan(fullStackSize));
		}

		[Test]
		public void Execute_ExactlyFillsTransitStack_WhenItemsMatchRemainingSpace() {
			const int fullStackSize = 10;
			this.fakeItem = new FakeItem(fullStackSize);
			ItemStack transitStack = this.fakeItem.CreateStack();
			this.CreateOperation(transitStack);

			IEnumerable<IItemContainer> containers = new IItemContainer[] {
				ContainerUtils.SubstituteContainer(
					this.fakeItem.CreateStack(),
					this.fakeItem.CreateStack(2),
					this.fakeItem.CreateStack(3),
					this.fakeItem.CreateStack(3)
				)
			};
			this.scope.GetOpenContainers().Returns(containers);

			Assert.That(this.operation.Execute(), Is.True);
			Assert.That(transitStack.quantity, Is.EqualTo(fullStackSize));
		}

		[Test]
		public void Execute_DecreasesSourceSlotQuantity_ByTransferredAmount() {
			const int fullStackSize = 10;
			this.fakeItem = new FakeItem(fullStackSize);
			ItemStack transitStack = this.fakeItem.CreateStack(5);
			this.CreateOperation(transitStack);

			ItemStack stack = this.fakeItem.CreateStack(7);
			const int shouldRemain = 2;

			IEnumerable<IItemContainer> containers = new IItemContainer[] {
				ContainerUtils.SubstituteContainer(stack)
			};
			this.scope.GetOpenContainers().Returns(containers);

			Assert.That(this.operation.Execute(), Is.True);
			Assert.That(transitStack.quantity, Is.EqualTo(fullStackSize));
			Assert.That(stack.quantity, Is.EqualTo(shouldRemain));
		}

		[Test]
		public void Execute_DoesNotModifySlots_WhenTransitStackAlreadyFull() {
			const int fullStackSize = 10;
			this.fakeItem = new FakeItem(fullStackSize);
			ItemStack transitStack = this.fakeItem.CreateStack(fullStackSize);
			this.CreateOperation(transitStack);

			ItemStack stack1 = this.fakeItem.CreateStack();
			ItemStack stack2 = this.fakeItem.CreateStack(6);
			ItemStack stack3 = this.fakeItem.CreateStack(2);

			IEnumerable<IItemContainer> containers = new IItemContainer[] {
				ContainerUtils.SubstituteContainer(stack1, stack2, stack3)
			};
			this.scope.GetOpenContainers().Returns(containers);

			Assert.That(this.operation.Execute(), Is.False);
			Assert.That(transitStack.quantity, Is.EqualTo(fullStackSize));
			Assert.That(stack1.quantity, Is.EqualTo(1));
			Assert.That(stack2.quantity, Is.EqualTo(6));
			Assert.That(stack3.quantity, Is.EqualTo(2));
		}

		[Test]
		public void Execute_HandlesMultipleSlotsAcrossMultipleContainers() {
			ItemStack transitStack = this.fakeItem.CreateStack();
			this.CreateOperation(transitStack);

			ItemStack stack1 = this.fakeItem.CreateStack();
			ItemStack stack2 = this.fakeItem.CreateStack();

			IItemContainer container1 = ContainerUtils.SubstituteContainer(stack1);
			IItemContainer container2 = ContainerUtils.SubstituteContainer(stack2);
			IEnumerable<IItemContainer> containers = new IItemContainer[] { container1, container2 };
			this.scope.GetOpenContainers().Returns(containers);

			Assert.That(this.operation.Execute(), Is.True);
			Assert.That(stack1.IsEmpty(), Is.True);
			Assert.That(stack2.IsEmpty(), Is.True);
		}
	}
}
