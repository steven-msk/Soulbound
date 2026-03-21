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
	public class SwapTransitTests : SingleSlotOperationTests<SwapTransit> {
		const int DEFAULT_FULL_STACK = 256;
		private FakeItem fakeItemA;
		private FakeItem fakeItemB;

		[SetUp]
		public void Setup() {
			scope = Substitute.For<IItemContainerScope>();
			fakeItemA = new FakeItem {
				_fullStackSize = DEFAULT_FULL_STACK
			};
			fakeItemB = new FakeItem {
				_fullStackSize = DEFAULT_FULL_STACK
			};
		}

		protected override SwapTransit GetOperation(IItemContainer container, int slotIndex, IItemContainerScope scope) {
			return new SwapTransit(container, slotIndex, scope);
		}

		[Test]
		public void CanExecute_SlotHasStack_And_TransitHasStack_ReturnsTrue() {
			CreateOperation(fakeItemA.CreateStack(), fakeItemB.CreateStack());

			Assert.That(operation.CanExecute(), Is.True);
		}

		[Test]
		public void CanExecute_EmptySlot_ReturnsFalse() {
			CreateOperation(fakeItemA.CreateStack(), null);

			Assert.That(operation.CanExecute(), Is.False);
		}

		[Test]
		public void CanExecute_NoTransitStack_ReturnsFalse() {
			CreateOperation(null, fakeItemB.CreateStack());

			Assert.That(operation.CanExecute(), Is.False);
		}


		[Test]
		public void Execute_SwapsStacksCorrectly() {
			ItemStack slotStack = fakeItemB.CreateStack();
			ItemStack transitStack = fakeItemA.CreateStack();
			CreateOperation(transitStack, slotStack);

			Assert.That(operation.Execute(), Is.True);
			Assert.That(scope.GetTransitStack(), Is.EqualTo(slotStack));
			Assert.That(slot.GetStack(), Is.EqualTo(transitStack));

			scope.Received().SetTransitStack(Arg.Is<ItemStack>(slotStack));
			slot.Received().SetStack(Arg.Is<ItemStack>(transitStack));
		}

		[Test]
		public void Execute_WhenCannotExecute_ReturnsFalse() {
			CreateOperation(null, null);

			Assert.That(operation.CanExecute(), Is.False);
		}
	}
}
