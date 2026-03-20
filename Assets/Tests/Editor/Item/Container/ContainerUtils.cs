using NSubstitute;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.ItemSystem.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal static class ContainerUtils {
	public static IItemContainer SubstituteContainer(params ItemStack[] stacks) {
		IItemContainer container = Substitute.For<IItemContainer>();
		container.GetSlotCount().Returns(stacks.Length);
		int[] slots = new int[stacks.Length];

		for (int i = 0; i < stacks.Length; i++) {
			IItemSlot slot = Substitute.For<IItemSlot>();
			slot.GetIndex().Returns(i);
			slot.GetStack().Returns(stacks[i]);
			slot.GetContainer().Returns(container);
			container.GetSlot(Arg.Is(i)).Returns(slot);
			slots[i] = i;
		}

		container.GetAllSlots().Returns(slots);

		return container;
	}
}
