using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Client.World.EntitySystem;
using SoulboundBackend.Client.World.EntitySystem.SpawnData;
using SoulboundBackend.Core;
using SoulboundBackend.Core.AssetManagement;
using SoulboundBackend.Core.Resource;
using System;
using TMPro;
using UnityEngine;
using Logger = SoulboundBackend.Common.Logging.Logger;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem {
	public class ItemStack {
		private static readonly Logger logger = Logger.CreateInstance();
		public event Action<int, int>? onQuantityChanged;
		public Item item { get; }
		public int quantity { get; private set; }
		public int MaxStackSize => item.maxStackSize;
		[Obsolete] public bool isDropped { get; private set; } = false;

		public ItemStack(Item item, int quantity) {
			this.item = item;
			this.quantity = Mathf.Min(quantity, item.maxStackSize);
		}

		[Obsolete]
		public void Drop(Vector2 pos, Vector2 dropForce, bool playerAction = false) {
			GameObject? worldPrefab = item.aspect.worldPrefabSupplier?.Invoke();
			if (item.aspect.worldPrefabSupplier == null) {
				logger.LogError(null, "No world prefab supplier for item {}. Using icon fallback.", item);
				worldPrefab = WorldPrefabFactory.GetInstantiator().Invoke();
			}
			GameObject droppedItem = worldPrefab!;
			DroppedItem pickup = droppedItem.GetComponent<DroppedItem>() ?? droppedItem.AddComponent<DroppedItem>();
			Soulbound.instance.GetActiveLevelManager().entityManager.Spawn(pickup, new DroppedItem.SpawnData() {
				 position = pos,
				 itemStack = this,
				 dropForce = dropForce,
				 pickupDelay = playerAction ? 2f : 0f
			});
			isDropped = true;
		}

		[Obsolete] public void OnPickedUp() => isDropped = false;

		public bool IsFull() => quantity >= item.maxStackSize;
		public bool IsEmpty() => quantity <= 0;

		public void SetQuantity(int amount) {
			int oldQuantity = quantity;
			quantity = Mathf.Min(amount, MaxStackSize);
			quantity = Mathf.Max(0, quantity);		// disallow negative quantity
			OnQuantityChanged(oldQuantity, quantity);
		}

		// Try to add items, return how many were actually added
		public int Increment(int amount = 1) {
			if (amount <= 0) return 0;

			int space = MaxStackSize - quantity;
			int added = Mathf.Min(space, amount);
			quantity += added;
			OnQuantityChanged(quantity - added, quantity);
			return added;
		}

		// Try to remove items, returns how many were actually removed
		public int Decrement(int amount = 1) {
			if (amount <= 0) return 0;

			int removed = Mathf.Min(quantity, amount);
			quantity -= removed;
			OnQuantityChanged(quantity + removed, quantity);
			return removed;
		}

		private void OnQuantityChanged(int old, int @new) {
			onQuantityChanged?.Invoke(old, @new);
		}

		// FEATUREIMPL: dropped item stacks converging to avoid lag
	}
}
