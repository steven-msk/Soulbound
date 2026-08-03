using SoulboundEngine.Client.Component;
using SoulboundEngine.Client.Interaction;
using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.World.Block;
using SoulboundEngine.Client.World.Entity;
using SoulboundEngine.Client.World.Level;
using SoulboundEngine.Core.Registry;
using System;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.Item {
	public struct ItemStack : IComponentHolder {
		private static MergedComponentMap cachedEmptyComponents = null!;
		private static MergedComponentMap CachedEmptyComponents => cachedEmptyComponents ??= MergedComponentMap.Create(Items.AIR.GetComponents(), ComponentChanges.EMPTY);
		public static readonly ItemStack EMPTY = new();
		private readonly MergedComponentMap components;
		private readonly Item item;
		public int count { get; private set; }

		// components are null on a default(ItemStack) instance
		// to guard against this, read this instead of this.components directly
		// if AssertComponentChangesNotOnEmpty is called, then its safe to use this.components
		private readonly MergedComponentMap ComponentsNonNull => this.components ?? CachedEmptyComponents;

		public ItemStack(IItemConvertible item)
			: this(item, 1) {
		}

		public ItemStack(IItemConvertible item, int count)
			: this(item, count, ComponentChanges.EMPTY){
		}

		public ItemStack(IItemConvertible item, int count, ComponentChanges componentChanges)
			: this (item, count, MergedComponentMap.Create(item.AsItem().GetComponents(), componentChanges)) {
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

		public readonly IComponentMap GetComponents() => this.ComponentsNonNull;

		public readonly ComponentChanges GetComponentChanges() => this.ComponentsNonNull.AsPatch();

		public readonly IComponentMap GetDefaultComponents() => this.GetItem().GetComponents();

		public readonly void ApplyChanges(ComponentChanges changes) {
			this.AssertComponentMutationNotOnEmpty();
			this.components.SetChanges(changes);
		}

		public readonly void SetComponentsFrom(IComponentMap map) {
			this.AssertComponentMutationNotOnEmpty();
			this.components.SetAll(map);
		}

		public readonly void Set<T>(ComponentType<T> type, T defaultValue, Func<T, T> applier) {
			this.AssertComponentMutationNotOnEmpty();
			this.components.Set(type, applier(this.components.GetOrDefault(type, defaultValue)));
		}

		public readonly void Set<T, U>(ComponentType<T> type, T defaultValue, U change, Func<T, U, T> applier) {
			this.AssertComponentMutationNotOnEmpty();
			this.components.Set(type, applier(this.components.GetOrDefault(type, defaultValue), change));
		}

		public readonly void ResetToDefaultComponents() {
			this.AssertComponentMutationNotOnEmpty();
			this.components.ClearChanges();
		}

		public readonly bool IsFull() => this.count >= this.item.GetMaxCount();
		public readonly bool IsEmpty() => this.count <= 0 || this.item == null;

		public readonly bool IsFullSize(int count) => count >= this.item.GetMaxCount();

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
			return this.item.GetMaxCount() - this.count;
		}

		public readonly bool IsOf(Item? item) {
			if (item == null) return this.IsEmpty();
			return Equals(item, this.item);
		}

		public static bool AreItemsEqual(ItemStack a, ItemStack b) {
			return a.IsOf(b.item) && b.IsOf(a.item);
		}

		public static bool AreItemsAndComponentsEqual(ItemStack a, ItemStack b) {
			return AreItemsEqual(a, b) && a.ComponentsNonNull.Equals(b.ComponentsNonNull);
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
			return this.CopyWithCount(this.item.GetMaxCount());
		}

		public ItemStack CopyAndEmpty() {
			ItemStack stack = this;
			this.count = 0;
			return stack;
		}

		public readonly ItemStack CopyComponentsToNewStack(IItemConvertible item, int count) {
			this.AssertComponentMutationNotOnEmpty();
			return new ItemStack(item, count, this.components.Copy());
		}

		public readonly void Copy<T>(ComponentType<T> type, IComponentsAccess from) {
			this.AssertComponentMutationNotOnEmpty();
			this.components.Set(type, from.Get(type));
		}

		public readonly T Set<T>(ComponentType<T> type, T? value) {
			this.AssertComponentMutationNotOnEmpty();
			if (value == null) return this.Remove(type);
			this.components.Set(type, value);
			return value;
		}

		public readonly T Remove<T>(ComponentType<T> type) {
			this.AssertComponentMutationNotOnEmpty();
			return this.components.Remove(type);
		}

		public void CapCount(int maxCount) {
			this.count = Mathf.Clamp(this.count, 0, maxCount);
		}

		public readonly Item GetItem() => this.item ?? Items.AIR;

		public readonly int GetMaxCount() => this.GetItem().GetMaxCount();

		public readonly int GetBreakLevel() {
			return this.ComponentsNonNull.GetOrDefault(ItemComponents.BREAK_LEVEL, this.GetItem().GetBreakLevel());
		}

		public static IActionResult OnPrimaryUse(ItemStack stack, Level level, PlayerEntity player, BlockPos blockPos) {
			return stack.GetItem().OnPrimaryUse(stack, level, player, blockPos);
		}
		public static IActionResult OnPrimaryUseOnBlock(ItemStack stack, BlockInteractionResult result) {
			return stack.GetItem().OnPrimaryUseOnBlock(result);
		}
		public static IActionResult OnPrimaryUseOnEntity(ItemStack stack, PlayerEntity player, Entity target) {
			return stack.GetItem().OnPrimaryUseOnEntity(stack, player, target);
		}
		public static IActionResult OnSecondaryUse(ItemStack stack, Level level, PlayerEntity player, BlockPos blockPos) {
			return stack.GetItem().OnSecondaryUse(stack, level, player, blockPos);
		}
		public static IActionResult OnSecondaryUseOnBlock(ItemStack stack, BlockInteractionResult result) {
			return stack.GetItem().OnSecondaryUseOnBlock(result);
		}
		public static IActionResult OnSecondaryUseOnEntity(ItemStack stack, PlayerEntity player, Entity target) {
			return stack.GetItem().OnSecondaryUseOnEntity(stack, player, target);
		}

		public readonly ItemStack OnItemUsed(InteractionType type, Level level, Entity user) {
			return this.GetItem().OnItemUsed(this, type, level, user);
		}

		public readonly ItemStack OnUseCanceled(InteractionType type, Level level, Entity user, int remainingTicks) {
			ItemStack stack = this.GetItem().OnUseCanceled(this, type, level, user, remainingTicks);
			return this.GetItem().OnUseCanceledOrFinished(stack, type, level, user, remainingTicks);
		}

		public readonly ItemStack OnUseTick(InteractionType type, Level level, Entity user, int remainingTicks) {
			return this.GetItem().OnUseTick(this, type, level, user, remainingTicks);
		}

		public readonly ItemStack OnUseFinished(InteractionType type, Level level, Entity user) {
			ItemStack stack = this.GetItem().OnUseFinished(this, type, level, user);
			return this.GetItem().OnUseCanceledOrFinished(stack, type, level, user, 0);
		}

		public readonly int GetUseTime(InteractionType type, Level level, Entity user) {
			return this.GetItem().GetUseTime(this, type, level, user);
		}

		private readonly void AssertComponentMutationNotOnEmpty() {
			if (this.IsEmpty()) throw new NotSupportedException("Cannot mutate components on empty stack");
		}

#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member

		[Obsolete("Cannot compare two item stacks with Equals", true)]
		public override bool Equals(object obj) {
			throw new NotSupportedException("Cannot compare two item stacks with Equals");
		}

		public readonly override int GetHashCode() {
			return HashCode.Combine(this.item, this.count);
		}

		public readonly override string ToString() {
			return this.IsEmpty() ? "EMPTY" : $"{this.item}[{this.count}]";
		}
	}
}
