using NSubstitute;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.ItemSystem.Container;
using System.Collections.Generic;

#nullable enable

internal abstract class SingleSlotOperationTests<TOperation> : SlotOperationTest where TOperation : SingleSlotOperation {
	protected IItemSlot slot = null!;

	protected void CreateOperation(ItemStack? transitStack, ItemStack? slotStack) {
		IItemContainer container = ContainerUtils.NewEmptyContainer(scope);
		CreateOperation(container, transitStack, slotStack);
	}

	protected void CreateOperation(IItemContainer container, ItemStack? transitStack, ItemStack? slotStack) {
		slot = container.AddSlot(slotStack);

		ContainerUtils.SubstituteTransit(transitStack, scope);

		operation = GetOperation(container, slot.GetIndex(), scope);
	}

	protected abstract TOperation GetOperation(IItemContainer container, int slotIndex, IItemContainerScope scope);
}
