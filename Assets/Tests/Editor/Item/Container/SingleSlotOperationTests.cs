using NSubstitute;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.ItemSystem.Container;
using System.Collections.Generic;

#nullable enable

public abstract class SingleSlotOperationTests<TOperation> where TOperation : SingleSlotOperation {
	protected IItemContainerScope scope;
	protected ISlotOperation operation;
	protected IItemSlot slot;

	protected void CreateOperation(ItemStack? transitStack, ItemStack? slotStack) {
		IItemContainer container = Substitute.For<IItemContainer>();

		slot = Substitute.For<IItemSlot>();
		slot.GetStack().Returns(slotStack);
		if (slotStack != null) slotStack.onQuantityChanged += QuantityChanged;
		slot.HasStack().Returns(slotStack != null);
		slot.When(s => s.SetStack(Arg.Any<ItemStack>())).Do(callInfo => {
			ItemStack? previousStack = slot.GetStack();
			if (previousStack != null) previousStack.onQuantityChanged -= QuantityChanged;

			ItemStack? stack = callInfo.Arg<ItemStack>();
			if (stack != null) stack.onQuantityChanged += QuantityChanged;

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

		operation = GetOperation(container, 0, scope);
	}

	private void QuantityChanged(int old, int @new) {
		if (@new <= 0) slot.SetStack(null);
	}

	protected abstract TOperation GetOperation(IItemContainer container, int slotIndex, IItemContainerScope scope);

}
