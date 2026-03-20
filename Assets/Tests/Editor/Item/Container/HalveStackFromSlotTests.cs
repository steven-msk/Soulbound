using NSubstitute;
using NUnit.Framework;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.ItemSystem.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;

namespace ItemTests.Container.Operations {
	public class HalveStackFromSlotTests : SingleSlotOperationTests<HalveStackFromSlot> {
		private const int DEFAULT_FULL_STACK = 256;
		private FakeItem fakeItem;

		[SetUp]
		public void Setup() {
			scope = Substitute.For<IItemContainerScope>();
			fakeItem = new FakeItem {
				_fullStackSize = DEFAULT_FULL_STACK,
			};
		}

		protected override HalveStackFromSlot GetOperation(IItemContainer container, int slotIndex, IItemContainerScope scope) {
			return new HalveStackFromSlot(container, slotIndex, scope);
		}

		[Test]
		public void CanExecute_ReturnsFalse_WhenSlotIsEmpty() {
			CreateOperation(null, null);
			Assert.That(operation.CanExecute(), Is.False);
		}

		[Test]
		public void CanExecute_ReturnsFalse_WhenTransitStackExists() {
			CreateOperation(fakeItem.CreateStack(), null);
			Assert.That(operation.CanExecute(), Is.False);
		}

		[Test]
		public void CanExecute_ReturnsTrue_WhenSlotHasStackAndNoTransit() {
			CreateOperation(null, fakeItem.CreateStack());
			Assert.That(operation.CanExecute(), Is.True);
		}

		[Test]
		public void Execute_ReturnsFalse_WhenCannotExecute() {
			CreateOperation(null, null);
			Assert.That(operation.Execute(), Is.False);
		}

		[Test]
		public void Execute_ReturnsTrue_OnSuccess() {
			CreateOperation(null, fakeItem.CreateStack());
			Assert.That(operation.Execute(), Is.True);
		}

		[Test]
		public void Execute_SetsTransitStack_WithCeilingHalf_WhenQuantityIsOdd() {
			CreateOperation(null, fakeItem.CreateStack(5));
			Assert.That(operation.Execute(), Is.True);
			Assert.That(scope.HasTransitStack(), Is.True);
			Assert.That(scope.GetTransitStack().quantity, Is.EqualTo(3));
		}

		[Test]
		public void Execute_SetsTransitStack_WithExactHalf_WhenQuantityIsEven() {
			CreateOperation(null, fakeItem.CreateStack(4));
			Assert.That(operation.Execute(), Is.True);
			Assert.That(scope.HasTransitStack(), Is.True);
			Assert.That(scope.GetTransitStack().quantity, Is.EqualTo(2));
		}

		[Test]
		public void Execute_DecrementsSlotStack_ByCeilingHalf_WhenQuantityIsOdd() {
			CreateOperation(null, fakeItem.CreateStack(5));
			Assert.That(operation.Execute(), Is.True);
			Assert.That(slot.HasStack(), Is.True);
			Assert.That(slot.GetStack().quantity, Is.EqualTo(2));
		}

		[Test]
		public void Execute_DecrementsSlotStack_ByExactHalf_WhenQuantityIsEven() {
			CreateOperation(null, fakeItem.CreateStack(4));
			Assert.That(operation.Execute(), Is.True);
			Assert.That(slot.HasStack(), Is.True);
			Assert.That(slot.GetStack().quantity, Is.EqualTo(2));
		}

		[Test]
		public void Execute_ClearsSlotStack_WhenQuantityIsOne() {
			CreateOperation(null, fakeItem.CreateStack());
			Assert.That(operation.Execute(), Is.True);
			Assert.That(slot.HasStack(), Is.False);
		}

		[Test]
		public void Execute_ClearsSlotStack_WhenFullStackSizeIsOne() {
			fakeItem._fullStackSize = 1;
			CreateOperation(null, fakeItem.CreateStack());
			Assert.That(operation.Execute(), Is.True);
			Assert.That(slot.HasStack(), Is.False);
		}

		[Test]
		public void Execute_SetsTransitStack_WithFullStack_WhenQuantityIsOne() {
			fakeItem._fullStackSize = 1;
			CreateOperation(null, fakeItem.CreateStack());
			Assert.That(operation.Execute(), Is.True);
			Assert.That(scope.HasTransitStack(), Is.True);
			Assert.That(scope.GetTransitStack().IsFull(), Is.True);
		}

		[Test]
		public void Execute_TransitStack_IsIndependentClone_OfSlotStack() {
			CreateOperation(null, fakeItem.CreateStack(10));
			Assert.That(operation.Execute(), Is.True);
			Assert.That(scope.HasTransitStack(), Is.True);

			scope.GetTransitStack().Increment(1);
			Assert.That(slot.GetStack().quantity, Is.EqualTo(5));
			Assert.That(scope.GetTransitStack().quantity, Is.EqualTo(6));
		}
	}
}
