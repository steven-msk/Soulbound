using NSubstitute;
using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.ItemSystem.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnityEngine.CullingGroup;


#nullable enable

internal static class ContainerUtils {
	public static IItemContainer SubstituteContainer(params ItemStack?[] stacks) {
		IItemContainer container = Substitute.For<IItemContainer>();
		container.GetSize().Returns(stacks.Length);
		int[] slots = new int[stacks.Length];

		for (int i = 0; i < stacks.Length; i++) {
			IItemSlot slot = Substitute.For<IItemSlot>();
			slot.GetIndex().Returns(i);
			slot.GetStack().Returns(stacks[i]);
			slot.GetContainer().Returns(container);
			container.GetSlot(i).Returns(slot);
			slots[i] = i;
		}

		container.GetAllSlots().Returns(slots);

		return container;
	}

	public static IItemContainer NewEmptyContainer(IItemContainerScope? scope) {
		IItemContainer container = Substitute.For<IItemContainer>();
		IReadOnlyList<int> slots = Array.Empty<int>();

		container.GetSlot(Arg.Any<int>()).Returns((IItemSlot?)null);
		container.GetSize().Returns(0);
		container.GetAllSlots().Returns(slots);

		if (scope != null) {
			IEnumerable<IItemContainer> existing = scope.GetOpenContainers();
			List<IItemContainer> containers = existing.ToList();
			containers.Add(container);
			scope.GetOpenContainers().Returns(containers);
		}

		return container;
	}

	public static IItemSlot AddSlot(this IItemContainer container, ItemStack? stack = null) {
		IItemSlot slot = SubstituteSlot(stack);
		container.AddSlot(slot);
		return slot;
	}

	public static IItemSlot SubstituteSlot(ItemStack? stack) {
		SubstitutedItemSlot slot = Substitute.For<SubstitutedItemSlot>(stack);
		slot.When(s => s.SetStack(Arg.Any<ItemStack>()))
			.Do(callInfo => slot.Internal_SetStack(callInfo.Arg<ItemStack?>()));
		return slot;
	}

	public static void AddSlot(this IItemContainer container, IItemSlot slot) {
		int index = container.GetSize();
		List<int> newIndices = container.GetAllSlots().ToList();
		newIndices.Add(index);

		container.GetSlot(index).Returns(slot);
		slot.GetContainer().Returns(container);
		slot.GetIndex().Returns(index);

		container.GetAllSlots().Returns(newIndices);
		container.GetSize().Returns(newIndices.Count);
	}

	public static void SubstituteTransit(ItemStack? stack, IItemContainerScope scope) {
		SubstitutedItemSlot superficialSlot = Substitute.For<SubstitutedItemSlot>(stack);

		scope.GetTransitStack().Returns(stack);
		scope.HasTransitStack().Returns(stack != null);

		scope.When(s => s.SetTransitStack(Arg.Any<ItemStack>())).Do(callInfo => {
			ItemStack? stack = callInfo.Arg<ItemStack?>();

			superficialSlot.Internal_SetStack(stack);

			scope.GetTransitStack().Returns(stack);
			scope.HasTransitStack().Returns(stack != null);
		});

		superficialSlot.quantityChanged += (_, _, @new) => {
			if (@new <= 0) scope.SetTransitStack(null);
		};
	}
}
