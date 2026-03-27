using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Common;
using SoulboundEngine.Core;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable enable

namespace SoulboundEngine.Client.ItemSystem.Container {
	public interface IItemSlot {
		event Action<ItemStack?> setStack;
		event Action<ItemStack?, ItemStack?>? stackChanged;
		event Action<ItemStack, int, int>? quantityChanged;

		ItemStack? GetStack();
		void SetStack(ItemStack? stack);

		int GetIndex();
		IItemContainer GetContainer();

		public bool HasStack() => GetStack()?.quantity > 0;

		public SlotRef GetRef() => new(GetContainer(), GetIndex());
	}
}
