using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.ItemSystem.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

public class SubstitutedItemSlot : IItemSlot {
	event Action<ItemStack?> IItemSlot.setStack {
		add => throw new NotImplementedException();
		remove => throw new NotImplementedException();
	}
	public event Action<ItemStack?, ItemStack?>? stackChanged;
	public event Action<ItemStack, int, int>? quantityChanged;

	private ItemStack? stack;

	public SubstitutedItemSlot(ItemStack? stack) {
		this.stack = stack;
	}

	public virtual IItemContainer GetContainer() => null!;

	public virtual int GetIndex() => 0;

	public ItemStack? GetStack() => stack;

	// intended empty implementation
	// NSubstitute will override any logic written here
	public virtual void SetStack(ItemStack? stack) {
	}

	internal void Internal_SetStack(ItemStack? stack) => this.stack = stack;

	internal void Internal_OnQuantityChanged(int old, int @new) {
		if (stack != null) {
			quantityChanged?.Invoke(stack, old, @new);
			if (@new <= 0) SetStack(null);
		}
	}

	internal void Internal_StackChanged(ItemStack? old, ItemStack? @new) {
		stackChanged?.Invoke(old, @new);
	}
}
