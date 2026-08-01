using SoulboundEngine.Client.Component;
using SoulboundEngine.Core.Registry;
using System;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.Item {
	public struct ItemStack : IComponentHolder {
		public static readonly ItemStack EMPTY = new();
		private readonly MergedComponentMap components;
		public readonly Item item;
		public int count { get; private set; }

		public ItemStack(IItemConvertible item)
			: this(item, 1) {
		}

		public ItemStack(IItemConvertible item, int count)
			: this(item, count, ComponentChanges.EMPTY){
		}

		public ItemStack(IItemConvertible item, int count, ComponentChanges componentChanges)
			: this (item, count, MergedComponentMap.Create(IComponentMap.EMPTY, componentChanges)) {
		}

		public ItemStack(RegistryEntry<Item> item) 
			: this(item, 1) {
		}

		public ItemStack(RegistryEntry<Item> item, int count) 
			: this(item.GetValue(), count) {
		}

		private ItemStack(IItemConvertible item, int count, MergedComponentMap components) {
			this.item = item.AsItem();
			this.count = count;
			this.components = components;
		}

		public readonly IComponentMap GetComponents() => this.components;

		public readonly ComponentChanges GetComponentChanges() => this.components.AsPatch();

		public readonly void ApplyChanges(ComponentChanges changes) {
			this.components.SetChanges(changes);
		}

		public readonly void SetComponentsFrom(IComponentMap map) {
			this.components.SetAll(map);
		}

		public readonly void Set<T>(ComponentType<T> type, T defaultValue, Func<T, T> applier) {
			this.components.Set(type, applier(this.components.GetOrDefault(type, defaultValue)));
		}

		public readonly void Set<T, U>(ComponentType<T> type, T defaultValue, U change, Func<T, U, T> applier) {
			this.components.Set(type, applier(this.components.GetOrDefault(type, defaultValue), change));
		}

		public readonly bool IsFull() => this.count >= this.item.fullStackSize;
		public readonly bool IsEmpty() => this.count <= 0 || this.item == null;

		public readonly bool IsFullSize(int count) => count >= this.item.fullStackSize;

		/// <summary>
		/// Try to add items. Returns how may were actually added.
		/// </summary>
		public int Increment(int amount = 1) {
			if (amount <= 0) return 0;

			int added = Mathf.Min(this.GetSpaceLeft(), amount);
			this.count += added;
			return added;
		}
		
		/// <summary>
		/// Try to remove items. Returns how many were actually removed
		/// </summary>
		public int Decrement(int amount = 1) {
			if (amount <= 0) return 0;

			int removed = Mathf.Min(this.count, amount);
			this.count -= removed;
			return removed;
		}

		public readonly int GetSpaceLeft() {
			if (this.IsOf(null)) return 0;
			return this.item.fullStackSize - this.count;
		}

		public readonly bool IsOf(Item? item) {
			if (item == null) return this.IsEmpty();
			return Equals(item, this.item);
		}

		public static bool AreItemsEqual(ItemStack a, ItemStack b) {
			return a.IsOf(b.item) && b.IsOf(a.item);
		}

		public static bool AreItemsAndComponentsEqual(ItemStack a, ItemStack b) {
			return AreItemsEqual(a, b) && a.components.Equals(b.components);
		}

		public static bool AreEqual(ItemStack a, ItemStack b) {
			return AreItemsEqual(a, b) && a.count == b.count && AreItemsAndComponentsEqual(a, b);
		}

		public void FillFrom(ref ItemStack itemStack) {
			if (!AreItemsEqual(itemStack, this)) return;

			int added = itemStack.Decrement(this.GetSpaceLeft());
			this.Increment(added);
			if (itemStack.IsEmpty()) itemStack = EMPTY;
		}

		public ItemStack Split(int amount) {
			int actualAmount = this.Decrement(amount);
			if (actualAmount <= 0) return EMPTY;
			return this.CopyWithCount(amount);
		}
	
		public readonly ItemStack CopyWithCount(int newCount) {
			if (this.IsOf(null)) return EMPTY;
			return new ItemStack(this.item, newCount, this.components.Copy());
		}

		public readonly ItemStack Copy() => this.CopyWithCount(this.count);

		public readonly ItemStack CopyFullStack() {
			if (this.IsOf(null)) return EMPTY;
			return this.CopyWithCount(this.item.fullStackSize);
		}

		public ItemStack CopyAndEmpty() {
			ItemStack stack = this;
			this.count = 0;
			return stack;
		}

		public readonly ItemStack CopyComponentsToNewStack(IItemConvertible item, int count) {
			return new ItemStack(item, count, this.components.Copy());
		}

		public void Copy<T>(ComponentType<T> type, IComponentsAccess from) {
			this.components.Set(type, from.Get(type));
		}

		public void CapCount(int maxCount) {
			this.count = Mathf.Clamp(this.count, 0, maxCount);
		}

		[Obsolete("Cannot compare two item stacks with Equals", true)]
		public override bool Equals(object obj) {
			throw new NotSupportedException("Cannot compare two item stacks with Equals");
		}

		public readonly override int GetHashCode() {
			return HashCode.Combine(this.item, this.count);
		}

		public readonly override string ToString() {
			return this.IsEmpty() ? "EMPTY" : $"{this.item}:{this.count}";
		}
	}
}
